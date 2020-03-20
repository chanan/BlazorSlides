using CSHTMLTokenizer.Tokens;
using System;

namespace BlazorSlides.Internal
{
    public class StringContent : IContent
    {
        public IToken Token { get; set; }
        public Guid Id { get; } = Guid.NewGuid();
    }
}