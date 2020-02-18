using System.Collections.Generic;

namespace BlazorSlides.Internal
{
    public interface ISlideContainer : ISlide
    {
        public List<IVerticalSlide> VerticalSlides { get; set; }
    }
}
