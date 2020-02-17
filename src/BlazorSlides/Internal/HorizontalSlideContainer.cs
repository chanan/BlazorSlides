using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorSlides.Internal
{
    class HorizontalSlideContainer : IHorizontalSlide, ISlideContainer
    {
        public int HorizontalIndex { get; set; }
        public List<IVerticalSlide> VerticalSlides { get; set; }
    }
}
