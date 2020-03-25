using System.Collections.Generic;

namespace BlazorSlides.Internal
{
    class InternalStack : ISlide
    {
        public int HorizontalIndex { get; set; }
        public int? VerticalIndex { get; set; }
        public List<InternalFragment> Fragments { get; set; } = new List<InternalFragment>();
        public List<InternalSlide> Slides { get; set; } = new List<InternalSlide>();
    }
}
