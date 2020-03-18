using CSHTMLTokenizer.Tokens;
using System;
using System.Collections.Generic;

namespace BlazorSlides.Internal
{
  public class FragmentContent : IContent
  {
        public List<IContent> Contents { get; set; }
        //public List<IToken> Tokens { get; set; } = new List<IToken>();
        public bool IsPast { get; set; }
        public bool IsCurrent { get; set; }
        public Guid Id { get; } = Guid.NewGuid();
    }
}