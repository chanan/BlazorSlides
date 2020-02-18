using BlazorSlides.Internal;
using CSHTMLTokenizer;
using CSHTMLTokenizer.Tokens;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Polished;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace BlazorSlides
{
    public partial class Slides : ComponentBase
    {
        private readonly ServiceProvider _emptyServiceProvider = new ServiceCollection().BuildServiceProvider();
        private readonly Func<string, string> _encoder = (string t) => t;

        //Main reveal container
        private string _revealContainer;
        private string _reveal;
        private string _center;

        //Slides container
        private string _slidesContainer;
        private string _slidesClass;
        private string _size;

        private readonly IMixins _mixins = new Mixins();

        //State
        private List<IHorizontalSlide> _slides = new List<IHorizontalSlide>();
        private int _countOfHorizontalSlide = 0;
        private int _countOfSlides = 0;
        private int _currentHorizontalIndex = 1;
        private int _currentVerticalIndex = 0;
        private int _CurrentNumberOfVerticalSlides = 0;
        private int _currentPastCount = 0;
        private bool _hasHorizontal = false;
        private bool _hasVertical = false;
        private bool _hasDarkBackground = false;
        private bool _hasLightBackground = true;
        private bool _showProgress = true;
        private bool _showSlideNumbers = true;

        [Parameter] public RenderFragment ChildContent { get; set; }

        protected override void OnParametersSet()
        {
            ProcessParameters();
        }

        //Event callbacks
        private void OnNext(MouseEventArgs e)
        {
            if (_currentHorizontalIndex != _slides.Count)
            {
                _currentHorizontalIndex++;
            }
            UpdateVerticalState();
            UpdatePastCount();
        }

        private void OnPrevious(MouseEventArgs e)
        {
            if (_currentHorizontalIndex != 1)
            {
                _currentHorizontalIndex--;
            }
            UpdateVerticalState();
            UpdatePastCount();
        }

        private void OnUp(MouseEventArgs e)
        {
            if (_currentVerticalIndex != 1)
            {
                _currentVerticalIndex--;
            }
            UpdatePastCount();
        }

        private void OnDown(MouseEventArgs e)
        {
            if (_currentVerticalIndex != _CurrentNumberOfVerticalSlides)
            {
                _currentVerticalIndex++;
            }
            UpdatePastCount();
        }

        private void ProcessParameters()
        {
            string content = RenderAsString();
            _slides = ParseSlides(content);
            _hasHorizontal = _slides.Count > 1;
            _hasVertical = _slides.Any(slide => slide is ISlideContainer);
            _countOfHorizontalSlide = _slides.Count;
            _countOfSlides = _slides.Sum(slide =>
            {
                if (slide is ISlideWithContent)
                {
                    return 1;
                }
                else
                {
                    return ((ISlideContainer)slide).VerticalSlides.Count;
                }
            });
            UpdateVerticalState();
        }

        private List<IHorizontalSlide> ParseSlides(string content)
        {
            List<IHorizontalSlide> list = new List<IHorizontalSlide>();
            List<Line> lines = Tokenizer.Parse(content);
            List<IToken> tokens = new List<IToken>();
            int horizontalIndex = 0;
            int verticalIndex = 0;
            bool horizontal = false;
            bool vertical = false;
            bool inTag = false;
            List<IVerticalSlide> verticalSlides = new List<IVerticalSlide>();
            foreach (Line line in lines)
            {
                foreach (IToken token in line.Tokens)
                {
                    if (horizontal && vertical && IsOpenSection(token))
                    {
                        throw new FormatException("Sections can only be two levels deep");
                    }
                    if (horizontal && !vertical && IsOpenSection(token))
                    {
                        tokens = new List<IToken>();
                        vertical = true;
                        inTag = true;
                    }
                    if (horizontal && !vertical && IsCloseSection(token))
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (IToken t in tokens)
                        {
                            sb.Append(t.ToHtml());
                        }
                        if (verticalSlides.Count > 0)
                        {
                            list.Add(new HorizontalSlideContainer
                            {
                                HorizontalIndex = ++horizontalIndex,
                                VerticalSlides = verticalSlides
                            });
                        }
                        else
                        {
                            list.Add(new HorizontalSlideContent
                            {
                                Content = sb.ToString(),
                                HorizontalIndex = ++horizontalIndex
                            });
                        }
                        horizontal = false;
                        tokens = new List<IToken>();
                        verticalSlides = new List<IVerticalSlide>();
                        verticalIndex = 0;
                        inTag = false;
                    }
                    if (horizontal && vertical && IsCloseSection(token))
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (IToken t in tokens)
                        {
                            sb.Append(t.ToHtml());
                        }
                        verticalSlides.Add(new VerticalSlide
                        {
                            Content = sb.ToString(),
                            VerticalIndex = ++verticalIndex,
                            HorizontalIndex = horizontalIndex + 1
                        });
                        vertical = false;
                        tokens = new List<IToken>();
                        inTag = false;
                    }
                    if (!horizontal && IsOpenSection(token))
                    {
                        tokens = new List<IToken>();
                        verticalSlides = new List<IVerticalSlide>();
                        horizontal = true;
                        vertical = false;
                        inTag = true;
                    }
                    if (!IsOpenSection(token) && !IsCloseSection(token) && inTag)
                    {
                        tokens.Add(token);
                    }
                }
            }
            return list;
        }

        private bool IsOpenSection(IToken token)
        {
            return token.TokenType == TokenType.StartTag && ((StartTag)token).Name == "section";
        }

        private bool IsCloseSection(IToken token)
        {
            return token.TokenType == TokenType.EndTag && ((EndTag)token).Name == "section";
        }

        private void UpdateVerticalState()
        {
            IHorizontalSlide currentSlide = _slides[_currentHorizontalIndex - 1];
            if (currentSlide is ISlideContainer)
            {
                _currentVerticalIndex = 1;
                _CurrentNumberOfVerticalSlides = ((ISlideContainer)currentSlide).VerticalSlides.Count;
            }
            else
            {
                _currentVerticalIndex = 0;
                _CurrentNumberOfVerticalSlides = 0;
            }
        }

        private void UpdatePastCount()
        {
            _currentPastCount = _slides.Sum(slide =>
            {
                switch (slide)
                {
                    case ISlideWithContent content:
                        return _currentHorizontalIndex > content.HorizontalIndex  ? 1 : 0;
                    case ISlideContainer stack:
                        if (_currentHorizontalIndex > stack.HorizontalIndex)
                        {
                            return stack.VerticalSlides.Count;
                        }
                        else if(_currentHorizontalIndex == stack.HorizontalIndex)
                        {
                            return _currentVerticalIndex - 1;
                        }
                        else
                        {
                            return 0;
                        }
                    default:
                        throw new ArgumentException("No such Slide should exist");
                }
            });
        }

        private string RenderAsString()
        {
            string result = string.Empty;
            try
            {
                ParameterView paramView = ParameterView.FromDictionary(new Dictionary<string, object>() { { "ChildContent", ChildContent } });
                using HtmlRenderer htmlRenderer = new HtmlRenderer(_emptyServiceProvider, NullLoggerFactory.Instance, _encoder);
                IEnumerable<string> tokens = GetResult(htmlRenderer.Dispatcher.InvokeAsync(() => htmlRenderer.RenderComponentAsync<TempComponent>(paramView)));
                result = string.Join("", tokens.ToArray());
            }
            catch
            {
                //ignored dont crash if can't get result
            }
            return result;
        }

        private IEnumerable<string> GetResult(Task<ComponentRenderedText> task)
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
