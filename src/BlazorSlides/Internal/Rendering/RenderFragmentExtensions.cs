using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace BlazorSlides.Internal.Rendering
{
    internal static class RenderFragmentExtensions
    {
        private static readonly ServiceProvider _emptyServiceProvider = new ServiceCollection().BuildServiceProvider();
        private static readonly Func<string, string> _encoder = (string t) => t;

        internal static string RenderAsString(this RenderFragment renderFragment)
        {
            string result = string.Empty;
            try
            {
                ParameterView paramView = ParameterView.FromDictionary(new Dictionary<string, object>() { { "ChildContent", renderFragment } });
                using HtmlRenderer htmlRenderer = new HtmlRenderer(_emptyServiceProvider, NullLoggerFactory.Instance, _encoder);
                IEnumerable<string> tokens = GetResult(htmlRenderer.Dispatcher.InvokeAsync(() => htmlRenderer.RenderComponentAsync<TempComponent>(paramView)));
                result = string.Join("", tokens.ToArray());
            }
            catch
            {
                //ignored don't crash if can't get result
            }
            return result;
        }

        private static IEnumerable<string> GetResult(Task<ComponentRenderedText> task)
        {
            if (task.IsCompleted && task.Status == TaskStatus.RanToCompletion && !task.IsFaulted && !task.IsCanceled)
            {
                return task.Result.Tokens;
            }
            else
            {
                ExceptionDispatchInfo.Capture(task.Exception).Throw();
                throw new InvalidOperationException("We will never hit this line");
            }
        }

        private class TempComponent : ComponentBase
        {
            [Parameter] public RenderFragment ChildContent { get; set; }

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                builder.AddContent(0, ChildContent);
            }
        }
    }
}
