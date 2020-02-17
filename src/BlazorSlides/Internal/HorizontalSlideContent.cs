using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorSlides.Internal
{
    class HorizontalSlideContent : IHorizontalSlide, ISlideWithContent
    {
        public int HorizontalIndex { get; set; }
        public String Content { get; set; }
    }
}
