using System.Collections.Generic;

namespace BlazorSlides.Internal
{
    internal interface ISlide
    {
        public int HorizontalIndex { get; set; }
        public int? VerticalIndex { get; set; }
    }
}
