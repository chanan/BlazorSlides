using System.Collections.Generic;

namespace BlazorSlides.Internal
{
    internal class InternalStack : ISlide
    {
        public int HorizontalIndex { get; set; }
        public int? VerticalIndex { get; set; }
        public IList<InternalSlide> Slides { get; set; } = new List<InternalSlide>();
    }
}
