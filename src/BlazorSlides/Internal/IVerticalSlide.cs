using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorSlides.Internal
{
    public interface IVerticalSlide : ISlideWithContent
    {
        public int VerticalIndex { get; set; }
    }
}
