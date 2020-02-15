using BlazorSlides.Internal;
using CSHTMLTokenizer;
using CSHTMLTokenizer.Tokens;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
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
        private string _slides;
        private string _size;

        private IMixins _mixins = new Mixins();
        private List<SlideItem> _slideItems = new List<SlideItem>();
        private int _currentSlide = 1;

        [Parameter] public RenderFragment ChildContent { get; set; }

        protected override void OnParametersSet()
        {
            ProcessParameters();
        }

        private void Next()
        {
            if(_currentSlide == _slideItems.Count)
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
                _currentSlide = _slideItems.Count;
            }
            else
            {
                _currentSlide--;
            }
        }

        private void ProcessParameters()
        {
            string content = RenderAsString();
            _slideItems = ParseSlides(content);
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

        private bool IsOpenDiv(IToken token)
        {
            if (token.TokenType == TokenType.StartTag && ((StartTag)token).Name == "section")
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
            if(token.TokenType == TokenType.EndTag && ((EndTag)token).Name == "section")
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
                if (startTag.Name != "section") return false;
                bool found = startTag.Attributes.Exists(attribute => attribute.TokenType == TokenType.Attribute && ((AttributeToken)attribute).Name == "data-blazorslide");
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
