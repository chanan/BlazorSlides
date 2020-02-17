using System.Collections.Generic;

namespace BlazorSlides.Internal
{
    internal class HorizontalSlideContainer : IHorizontalSlide, ISlideContainer
    {
        public int HorizontalIndex { get; set; }
        public List<IVerticalSlide> VerticalSlides { get; set; }
    }
}
