using BlazorSlides.Internal;
using CSHTMLTokenizer;
using CSHTMLTokenizer.Tokens;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
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

        private List<SlideItem> _slides = new List<SlideItem>();

        //Css Classes
        private string _slideshowContainer;
        private string _prev;
        private string _rightSide;
        private string _next;
        private string _dot;
        private string _active;
        private string _dotActive;
        private string _dotsContainer;

        private int _currentSlide = 1;

        [Parameter] public RenderFragment ChildContent { get; set; }

        protected override void OnParametersSet()
        {
            ProcessParameters();
        }

        private void Next()
        {
            if(_currentSlide == _slides.Count)
            {
                _currentSlide = 1;
            }
            else
            {
                _currentSlide++;
            }
        }

        private void Previous()
        {
            if (_currentSlide == 1)
            {
                _currentSlide = _slides.Count;
            }
            else
            {
                _currentSlide--;
            }
        }

        private string GetDotClass(int index)
        {
            if(index == _currentSlide)
            {
                return _dotActive;
            }
            else
            {
                return _dot;
            }
        }
        private void DotClick(int newIndex)
        {
            _currentSlide = newIndex;
        }

        private void ProcessParameters()
        {
            string content = RenderAsString();
            _slides = ParseSlides(content);
        }

        private List<SlideItem> ParseSlides(string content)
        {
            List<SlideItem> list = new List<SlideItem>();
            List<Line> lines = Tokenizer.Parse(content);
            List<IToken> tokens = new List<IToken>();
            bool open = false;
            int divNum = 0;
            int index = 0;
            bool slideTag = false;
            string caption = null;
            foreach(var line in lines)
            {
                foreach(var token in line.Tokens)
                {
                    if (!open && IsOpenSlide(token))
                    {
                        tokens = new List<IToken>();
                        divNum = 0;
                        open = true;
                        slideTag = true;
                        caption = GetCaption(token);
                    }
                    if(open && IsOpenDiv(token))
                    {
                        divNum++;
                    }
                    if(open && IsCloseDiv(token))
                    {
                        divNum--;
                    }
                    if(open && IsCloseDiv(token) && divNum == 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach(var t in tokens)
                        {
                            sb.Append(t.ToHtml());
                        }
                        list.Add(new SlideItem
                        {
                            Content = sb.ToString(),
                            Index = ++index,
                            Caption = caption
                        }); ;
                        open = false;
                        slideTag = true;
                        caption = null;
                    }
                    if(!slideTag)
                    {
                        tokens.Add(token);
                    } 
                    else
                    {
                        slideTag = false;
                    }
                }
            }
            return list;
        }

        private string GetCaption(IToken token)
        {
            string ret = null;
            StartTag startTag = (StartTag)token;
            foreach(IToken attr in startTag.Attributes)
            {
                if(((AttributeToken)attr).Name == "data-blazorslide-caption")
                {
                    string caption = ((AttributeToken)attr).Value.ToString();
                    ret = caption.Substring(1, caption.Length - 2);
                }
            }
            return ret;
        }

        private bool IsOpenDiv(IToken token)
        {
            if (token.TokenType == TokenType.StartTag && ((StartTag)token).Name == "div")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsCloseDiv(IToken token)
        {
            if(token.TokenType == TokenType.EndTag && ((EndTag)token).Name == "div")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool IsOpenSlide(IToken token)
        {
            if(token.TokenType == TokenType.StartTag)
            {
                StartTag startTag = (StartTag)token;
                if (startTag.Name != "div") return false;
                bool found = false;
                foreach(IToken attribute in startTag.Attributes)
                {
                    if(attribute.TokenType == TokenType.Attribute && ((AttributeToken)attribute).Name == "data-blazorslide")
                    {
                        found = true;
                    }
                }
                return found;
            }
            else
            {
                return false;
            }
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
