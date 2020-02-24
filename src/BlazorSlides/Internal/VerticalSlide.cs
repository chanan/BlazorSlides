namespace BlazorSlides.Internal
{
    internal class VerticalSlide : SlideWithContent, ISlide, IVerticalSlide, ISlideWithContent
    {
        public int VerticalIndex { get; set; }
        public int HorizontalIndex { get; set; }
  }
}
