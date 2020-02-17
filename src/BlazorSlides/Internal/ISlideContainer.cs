using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorSlides.Internal
{
    public interface ISlideContainer
    {
        public List<IVerticalSlide> VerticalSlides { get; set; }
    }
}
