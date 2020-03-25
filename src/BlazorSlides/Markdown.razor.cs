using Microsoft.AspNetCore.Components;
using Markdig;
using BlazorSlides.Internal.Rendering;
using System;
using System.Text;

namespace BlazorSlides
{
    public partial class Markdown : ComponentBase
    {
        //Parameters
        [Parameter] public RenderFragment ChildContent { get; set; }
        private MarkupString Content => (MarkupString)Markdig.Markdown.ToHtml(FixTabs(ChildContent.RenderAsString()), new MarkdownPipelineBuilder().UseAdvancedExtensions().Build());

        private string FixTabs(string str)
        {
            StringBuilder ignored = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            string[] lines = str.Split(Environment.NewLine);
            int firstLine = 0;
            for(int i = 0; i < lines.Length; i++)
            {
                if(lines[i].Trim() != string.Empty)
                {
                    firstLine = i;
                    break;
                }
            }
            
            foreach (char ch in lines[firstLine])
            {
                if (char.IsWhiteSpace(ch))
                {
                    ignored.Append(ch);
                }
                else
                {
                    break;
                }
            }

            for (int i = firstLine; i < lines.Length; i++)
            {
                string line = lines[i];
                if (line.StartsWith(ignored.ToString()))
                {
                    string content = ReplaceFirst(line, ignored.ToString());
                    sb.AppendLine(content);
                }
            }

            return sb.ToString();
        }

        private string ReplaceFirst(string text, string search)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + text.Substring(pos + search.Length);
        }
    }
}
