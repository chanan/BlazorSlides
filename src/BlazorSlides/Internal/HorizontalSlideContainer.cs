using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorSlides.Internal
{
    class HorizontalSlideContainer : IHorizontalSlide
    {
        public int HorizontalIndex { get; set; }
        public List<VerticalSlide> VerticalSlides { get; set; }
    }
}
