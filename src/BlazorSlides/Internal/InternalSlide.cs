using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace BlazorSlides.Internal
{
    internal class InternalSlide : ISlide
    {
        public int HorizontalIndex { get; set; }
        public int? VerticalIndex { get; set; }
        public IList<InternalFragment> Fragments { get; set; } = new List<InternalFragment>();
        public string Id { get; set; }
        public ElementReference ElementReference { get; set; }
        public int Top { get; set; }
    }
}
