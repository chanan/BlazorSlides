using CSHTMLTokenizer.Tokens;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorSlides.Internal.Components
{
    public class ContentWriter : ComponentBase
    {
        [Parameter] public string FragmentClass { get; set; }
        [Parameter] public IEnumerable<IContent> Contents { get; set; }
        [Parameter] public int CurrentHorizontalIndex { get; set; }
        [Parameter] public int CurrentVerticalIndex { get; set; }

        int i = 0;
        private int Next()
        {
            return ++i;
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            List<IContent> list = Contents.ToList();
            foreach (IContent content in list)
            {
                switch (content)
                {
                    case StringContent sc:
                        RenderToken(builder, sc.Token, sc.Id);
                        break;
                    case FragmentContent fc:
                        builder.OpenComponent<Fragment>(Next());
                        builder.SetKey(fc.Id);
                        builder.AddAttribute(Next(), "FragmentContent", content);
                        builder.AddAttribute(Next(), "CurrentHorizontalIndex", CurrentHorizontalIndex);
                        builder.AddAttribute(Next(), "CurrentVerticalIndex", CurrentVerticalIndex);
                        builder.CloseComponent();
                        break;
                    case MarkdownContent mc:
                        builder.OpenRegion(Next());
                        builder.AddMarkupContent(Next(), mc.Content);
                        builder.CloseRegion();
                        break;
                }
            }
        }

        private void RenderToken(RenderTreeBuilder builder, IToken Token, Guid id)
        {
            switch (Token)
            {
                case Text text:
                    builder.AddContent(Next(), text.ToString());
                    break;
                case StartTag startTag:
                    if (startTag.LineType == LineType.SingleLine || startTag.LineType == LineType.MultiLineStart)
                    {
                        builder.OpenElement(Next(), startTag.Name);
                        builder.SetKey(id);
                    }

                    if (startTag.Attributes.Count > 0)
                    {
                        foreach (IToken token in startTag.Attributes)
                        {
                            AttributeToken attribute = (AttributeToken)token;
                            if(attribute.NameOnly)
                            {
                                builder.AddAttribute(Next(), attribute.Name, (string)null);
                            }
                            else
                            {
                                if(attribute.Name == "class" && attribute.Value.Content == "fragment")
                                {
                                    builder.AddAttribute(Next(), "class", FragmentClass);
                                } 
                                else
                                {
                                    builder.AddAttribute(Next(), attribute.Name, attribute.Value.Content);
                                }
                            }
                        }
                    }

                    if (startTag.LineType == LineType.SingleLine || startTag.LineType == LineType.MultiLineEnd)
                    {
                        if (startTag.IsSelfClosingTag && !startTag.IsGeneric)
                        {
                            builder.CloseElement();
                        }
                    }
                    break;
                case EndTag _:
                    builder.CloseElement();
                    break;
                default:
                    break;
            }
        }
    }
}
