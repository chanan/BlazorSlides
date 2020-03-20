using Markdig;
using Microsoft.AspNetCore.Components;
using System;

namespace BlazorSlides.Internal
{
    internal class MarkdownContent : IContent
    {
        public Guid Id { get; } = Guid.NewGuid();

        private string _content;
        public string Content
        {
            get
            {
                return _content;
            }

            set
            {
                _content = Markdown.ToHtml(value, new MarkdownPipelineBuilder().UseAdvancedExtensions().Build());
            }
        }
    }
}
