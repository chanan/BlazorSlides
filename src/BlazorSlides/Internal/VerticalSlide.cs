using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorSlides.Internal
{
    class VerticalSlide : ISlide, IVerticalSlide, ISlideWithContent
    {
        public int VerticalIndex { get; set; }
        public int HorizontalIndex { get; set; }
        public string Content { get; set; }
    }
}
