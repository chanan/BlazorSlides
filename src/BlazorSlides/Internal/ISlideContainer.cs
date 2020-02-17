using System.Collections.Generic;

namespace BlazorSlides.Internal
{
    public interface ISlideContainer
    {
        public List<IVerticalSlide> VerticalSlides { get; set; }
    }
}
