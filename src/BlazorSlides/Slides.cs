using BlazorSlides.Internal;
using BlazorSlides.Internal.Components;
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
        //Variables for RenderAsString
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

        //JsInterop
        private Scripts _scripts;
        private ElementReference _domWrapper;

        //Variables for JSInterop
        private int _offsetWidth = 0;

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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await _scripts.SetInstance();
                await UpdateJsInteropVars();
            }
        }

        //Scripts Events
        private async Task _OnWindowResize(object ignored)
        {
            await UpdateJsInteropVars();
        }

        async Task UpdateJsInteropVars()
        {
            _offsetWidth = await _scripts.GetOffsetWidth();
        }

        //Event callbacks
        private async Task OnNext(MouseEventArgs e)
        {
          if(NextFragment() == false)
          {
            if (_currentHorizontalIndex != _slides.Count)
            {
                _currentHorizontalIndex++;
            }
            UpdateVerticalState();
            UpdatePastCount();
          }
          await UpdateJsInteropVars();
        }

        private bool NextFragment() {
          SlideWithContent slide = (SlideWithContent)GetCurrentSlide();
          return slide.NextFragment();
        }

        private async Task OnPrevious(MouseEventArgs e)
        {
          if(PreviousFragment() == false)
          {
            if (_currentHorizontalIndex != 1)
            {
                _currentHorizontalIndex--;
            }
            UpdateVerticalState();
            UpdatePastCount();
          }
          await UpdateJsInteropVars();
        }

        private bool PreviousFragment()
        {
          SlideWithContent slide = (SlideWithContent)GetCurrentSlide();
          return slide.PreviousFragment();
        }

        private async Task OnUp(MouseEventArgs e)
        {
            if (_currentVerticalIndex != 1)
            {
                _currentVerticalIndex--;
            }
            UpdatePastCount();
            await UpdateJsInteropVars();
        }

        private async Task OnDown(MouseEventArgs e)
        {
            if (_currentVerticalIndex != _CurrentNumberOfVerticalSlides)
            {
                _currentVerticalIndex++;
            }
            UpdatePastCount();
            await UpdateJsInteropVars();
        }

        private ISlide GetCurrentSlide() {
          IHorizontalSlide horizontalSlide = _slides[_currentHorizontalIndex - 1];
          if(horizontalSlide is HorizontalSlideContent) {
            return horizontalSlide;
          } else {
            HorizontalSlideContainer container = (HorizontalSlideContainer) horizontalSlide;
            return container.VerticalSlides[_currentVerticalIndex - 1];
          }
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
            int textareaCount = 0;
            bool horizontal = false;
            bool vertical = false;
            bool inTag = false;
            bool hasMarkdown = true;
            bool inMarkdown = false;
            List<IVerticalSlide> verticalSlides = new List<IVerticalSlide>();
            foreach (Line line in lines)
            {
                foreach (IToken token in line.Tokens)
                {
                    if(hasMarkdown && !inMarkdown && IsOpenMarkdown(token))
                    {
                        inMarkdown = true;
                        textareaCount = 0;
                    }
                    else if (inMarkdown && IsOpenTextArea(token))
                    {
                        textareaCount++;
                    }
                    if (inMarkdown && IsCloseTextArea(token))
                    {
                        textareaCount--;
                        if (textareaCount == -1)
                        {
                            inMarkdown = false;
                        }
                    }
                    if (inMarkdown)
                    {
                        tokens.Add(token);
                    }
                    if(!inMarkdown)
                    {
                        if (horizontal && vertical && IsOpenSection(token))
                        {
                            throw new FormatException("Sections can only be two levels deep");
                        }
                        if (horizontal && !vertical && IsOpenSection(token))
                        {
                            hasMarkdown = SectionHasMarkdown((StartTag)token);
                            tokens = new List<IToken>();
                            vertical = true;
                            inTag = true;
                        }
                        if (horizontal && !vertical && IsCloseSection(token))
                        {
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
                                HorizontalSlideContent slide = new HorizontalSlideContent
                                {
                                    HorizontalIndex = ++horizontalIndex
                                };
                                slide.Contents = GetSlideContents(tokens);
                                list.Add(slide);
                            }
                            horizontal = false;
                            tokens = new List<IToken>();
                            verticalSlides = new List<IVerticalSlide>();
                            verticalIndex = 0;
                            inTag = false;
                            hasMarkdown = false;
                        }
                        if (horizontal && vertical && IsCloseSection(token))
                        {
                            VerticalSlide slide = new VerticalSlide
                            {
                                VerticalIndex = ++verticalIndex,
                                HorizontalIndex = horizontalIndex + 1
                            };
                            slide.Contents = GetSlideContents(tokens);
                            verticalSlides.Add(slide);
                            vertical = false;
                            tokens = new List<IToken>();
                            inTag = false;
                            hasMarkdown = false;
                        }
                        if (!horizontal && IsOpenSection(token))
                        {
                            hasMarkdown = SectionHasMarkdown((StartTag)token);
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
                tokens.Add(new Text(Environment.NewLine));
            }
            return list;
        }

        private List<IContent> GetSlideContents(List<IToken> tokens) {
            bool isFragment = false;
            bool isMarkdown = false;
            int tagCount = 0;
            int textareaCount = 0;
            List<IToken> fragmentList = new List<IToken>();
            List<IToken> markdownList = new List<IToken>();
            List<IContent> list = new List<IContent>();
            foreach (IToken token in tokens)
            {
                switch (token.TokenType)
                {
                    case TokenType.StartTag:
                    {
                        if (isFragment)
                        {
                            fragmentList.Add(token);
                            tagCount++;
                        }
                        if (!isFragment && !isMarkdown && IsStartTagFragment((StartTag)token))
                        {
                            isFragment = true;
                            fragmentList = new List<IToken> { token };
                            tagCount = 0;
                        }
                        else if (!isMarkdown && IsOpenMarkdown(token))
                        {
                            isMarkdown = true;
                            textareaCount = 0;
                        }
                        else if (isMarkdown && IsOpenTextArea(token))
                        {
                            textareaCount++;
                        }
                        else
                        {
                            if (isMarkdown)
                            {
                                markdownList.Add(token);
                            }
                            else
                            {
                                list.Add(new StringContent { Token = token });
                            }
                        }
                        break;
                    }
                    case TokenType.EndTag:
                    {
                        if (!isMarkdown && isFragment)
                        {
                            tagCount--;
                            fragmentList.Add(token);
                            if (tagCount == -1)
                            {
                                isFragment = false;
                                list.Add(new FragmentContent { Contents = TransformFragments(fragmentList) });
                                fragmentList = new List<IToken>();
                                tagCount = 0;
                            }
                        }
                        else if(isMarkdown)
                        {
                            if(IsCloseTextArea(token))
                            {
                                textareaCount--;
                                if(textareaCount == -1)
                                {
                                    isMarkdown = false;
                                    StringBuilder sb = new StringBuilder();
                                    foreach (IToken t in markdownList)
                                    {
                                        sb.Append(t.ToHtml());
                                    }
                                    list.Add(new MarkdownContent { Content = FixTabs(sb.ToString()) });
                                }
                                else
                                {
                                    markdownList.Add(token);
                                }                                
                            }
                            else
                            {
                                markdownList.Add(token);
                            }
                        }
                        else
                        {
                            list.Add(new StringContent { Token = token });
                        }
                        break;
                    }
                    default:
                    {
                        if (isFragment)
                        {
                            fragmentList.Add(token);
                        }
                        else if(isMarkdown)
                        {
                            markdownList.Add(token);
                        }
                        else
                        {
                            list.Add(new StringContent { Token = token });
                        }
                        break;
                    }
                }
            }
            return list;
        }

        private string FixTabs(string str)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder ignored = new StringBuilder();
            var lines = str.Split(Environment.NewLine);
            bool foundFirstLine = false;
            foreach (string line in lines)
            {
                if(!foundFirstLine && line.Trim() == String.Empty)
                {
                    continue;
                }
                foundFirstLine = true;
                bool foundChar = false;
                foreach (char ch in line)
                {
                    if (!foundChar && char.IsWhiteSpace(ch))
                    {
                        ignored.Append(ch);
                    }
                    else
                    {
                        foundChar = true;
                    }
                }
                break;
            }
            if (ignored.Length > 0)
            {
                foreach (string line in lines)
                {
                    string trimmed = line.Replace(ignored.ToString(), String.Empty).TrimEnd();
                    sb.AppendLine(trimmed);
                }
            }
            else
            {
                sb.Append(str);
            }
            return sb.ToString();
        }

        private bool IsOpenTextArea(IToken token)
        {
            return token.TokenType == TokenType.StartTag && ((StartTag)token).Name == "textarea";
        }

        private bool IsCloseTextArea(IToken token)
        {
            return token.TokenType == TokenType.EndTag && ((EndTag)token).Name == "textarea";
        }

        private bool IsOpenMarkdown(IToken token)
        {
            return token.TokenType == TokenType.StartTag && ((StartTag)token).Name == "textarea"
                && ((StartTag)token).Attributes.Any(attr => ((AttributeToken)attr).NameOnly && ((AttributeToken)attr).Name == "data-template");
        }

        private bool SectionHasMarkdown(StartTag startTag)
        {
            return startTag.Attributes.Any(token => ((AttributeToken)token).Name.ToLower() == "data-markdown");
        }

        private List<IContent> TransformFragments(List<IToken> tokens)
        {
            List<IContent> list = new List<IContent>();
            foreach(IToken token in tokens)
            {
                list.Add(new StringContent { Token = token });
            }
            return list;
        }

        private bool IsStartTagFragment(StartTag startTag) {
          return startTag.Attributes.Any(token => ((AttributeToken)token).Name.ToLower() == "class" &&  ((AttributeToken)token).Value.Content.ToLower() == "fragment" );
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
                //ignored don't crash if can't get result
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
