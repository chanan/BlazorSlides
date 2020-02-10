﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace BlazorSlides.Internal
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "BL0006:Do not use RenderTree types", Justification = "Preview version")]
    internal class HtmlRenderer : Renderer
    {
        private static readonly HashSet<string> SelfClosingElements = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "area", "base", "br", "col", "embed", "hr", "img", "input", "link", "meta", "param", "source", "track", "wbr"
        };

        private readonly Func<string, string> _htmlEncoder;

        /// <summary>
        /// Initializes a new instance of <see cref="HtmlRenderer"/>.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to use to instantiate components.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
        /// <param name="htmlEncoder">A <see cref="Func{T, TResult}"/> that will HTML encode the given string.</param>
        public HtmlRenderer(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, Func<string, string> htmlEncoder)
            : base(serviceProvider, loggerFactory)
        {
            _htmlEncoder = htmlEncoder;
        }

        public override Dispatcher Dispatcher { get; } = Dispatcher.CreateDefault();

        /// <inheritdoc />
        protected override Task UpdateDisplayAsync(in RenderBatch renderBatch)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Renders a component into a sequence of <see cref="string"/> fragments that represent the textual representation
        /// of the HTML produced by the component.
        /// </summary>
        /// <param name="componentType">The type of the <see cref="IComponent"/>.</param>
        /// <param name="initialParameters">A <see cref="ParameterView"/> with the initial parameters to render the component.</param>
        /// <returns>A <see cref="Task"/> that on completion returns a sequence of <see cref="string"/> fragments that represent the HTML text of the component.</returns>
        public async Task<ComponentRenderedText> RenderComponentAsync(Type componentType, ParameterView initialParameters)
        {
            (int componentId, ArrayRange<RenderTreeFrame> frames) = await CreateInitialRenderAsync(componentType, initialParameters);

            List<string> result = new List<string>();
            int newPosition = RenderFrames(result, frames, 0, frames.Count);
            Debug.Assert(newPosition == frames.Count);
            return new ComponentRenderedText(componentId, result);
        }

        /// <summary>
        /// Renders a component into a sequence of <see cref="string"/> fragments that represent the textual representation
        /// of the HTML produced by the component.
        /// </summary>
        /// <typeparam name="TComponent">The type of the <see cref="IComponent"/>.</typeparam>
        /// <param name="initialParameters">A <see cref="ParameterView"/> with the initial parameters to render the component.</param>
        /// <returns>A <see cref="Task"/> that on completion returns a sequence of <see cref="string"/> fragments that represent the HTML text of the component.</returns>
        public Task<ComponentRenderedText> RenderComponentAsync<TComponent>(ParameterView initialParameters) where TComponent : IComponent
        {
            return RenderComponentAsync(typeof(TComponent), initialParameters);
        }

        /// <inheritdoc />
        protected override void HandleException(Exception exception)
        {
            ExceptionDispatchInfo.Capture(exception).Throw();
        }

        private int RenderFrames(List<string> result, ArrayRange<RenderTreeFrame> frames, int position, int maxElements)
        {
            int nextPosition = position;
            int endPosition = position + maxElements;
            while (position < endPosition)
            {
                nextPosition = RenderCore(result, frames, position, maxElements);
                if (position == nextPosition)
                {
                    throw new InvalidOperationException("We didn't consume any input.");
                }
                position = nextPosition;
            }

            return nextPosition;
        }

        private int RenderCore(
            List<string> result,
            ArrayRange<RenderTreeFrame> frames,
            int position,
            int length)
        {
            ref RenderTreeFrame frame = ref frames.Array[position];
            switch (frame.FrameType)
            {
                case RenderTreeFrameType.Element:
                    return RenderElement(result, frames, position);
                case RenderTreeFrameType.Attribute:
                    return RenderAttributes(result, frames, position, 1);
                case RenderTreeFrameType.Text:
                    result.Add(_htmlEncoder(frame.TextContent));
                    return ++position;
                case RenderTreeFrameType.Markup:
                    result.Add(frame.MarkupContent);
                    return ++position;
                case RenderTreeFrameType.Component:
                    return RenderChildComponent(result, frames, position);
                case RenderTreeFrameType.Region:
                    return RenderFrames(result, frames, position + 1, frame.RegionSubtreeLength - 1);
                case RenderTreeFrameType.ElementReferenceCapture:
                case RenderTreeFrameType.ComponentReferenceCapture:
                    return ++position;
                default:
                    throw new InvalidOperationException($"Invalid element frame type '{frame.FrameType}'.");
            }
        }

        private int RenderChildComponent(
            List<string> result,
            ArrayRange<RenderTreeFrame> frames,
            int position)
        {
            ref RenderTreeFrame frame = ref frames.Array[position];
            ArrayRange<RenderTreeFrame> childFrames = GetCurrentRenderTreeFrames(frame.ComponentId);
            RenderFrames(result, childFrames, 0, childFrames.Count);
            return position + frame.ComponentSubtreeLength;
        }

        private int RenderElement(
            List<string> result,
            ArrayRange<RenderTreeFrame> frames,
            int position)
        {
            ref RenderTreeFrame frame = ref frames.Array[position];
            result.Add("<");
            result.Add(frame.ElementName);
            int afterAttributes = RenderAttributes(result, frames, position + 1, frame.ElementSubtreeLength - 1);
            int remainingElements = frame.ElementSubtreeLength + position - afterAttributes;
            if (remainingElements > 0)
            {
                result.Add(">");
                int afterElement = RenderChildren(result, frames, afterAttributes, remainingElements);
                result.Add("</");
                result.Add(frame.ElementName);
                result.Add(">");
                Debug.Assert(afterElement == position + frame.ElementSubtreeLength);
                return afterElement;
            }
            else
            {
                if (SelfClosingElements.Contains(frame.ElementName))
                {
                    result.Add(" />");
                }
                else
                {
                    result.Add(">");
                    result.Add("</");
                    result.Add(frame.ElementName);
                    result.Add(">");
                }
                Debug.Assert(afterAttributes == position + frame.ElementSubtreeLength);
                return afterAttributes;
            }
        }

        private int RenderChildren(List<string> result, ArrayRange<RenderTreeFrame> frames, int position, int maxElements)
        {
            if (maxElements == 0)
            {
                return position;
            }

            return RenderFrames(result, frames, position, maxElements);
        }

        private int RenderAttributes(
            List<string> result,
            ArrayRange<RenderTreeFrame> frames, int position, int maxElements)
        {
            if (maxElements == 0)
            {
                return position;
            }

            for (int i = 0; i < maxElements; i++)
            {
                int candidateIndex = position + i;
                ref RenderTreeFrame frame = ref frames.Array[candidateIndex];
                if (frame.FrameType != RenderTreeFrameType.Attribute)
                {
                    return candidateIndex;
                }

                switch (frame.AttributeValue)
                {
                    case bool flag when flag:
                        result.Add(" ");
                        result.Add(frame.AttributeName);
                        break;
                    case string value:
                        result.Add(" ");
                        result.Add(frame.AttributeName);
                        result.Add("=");
                        result.Add("\"");
                        result.Add(_htmlEncoder(value));
                        result.Add("\"");
                        break;
                    default:
                        break;
                }
            }

            return position + maxElements;
        }

        private async Task<(int, ArrayRange<RenderTreeFrame>)> CreateInitialRenderAsync(Type componentType, ParameterView initialParameters)
        {
            IComponent component = InstantiateComponent(componentType);
            int componentId = AssignRootComponentId(component);

            await RenderRootComponentAsync(componentId, initialParameters);

            return (componentId, GetCurrentRenderTreeFrames(componentId));
        }
    }
}
