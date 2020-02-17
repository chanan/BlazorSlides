namespace BlazorSlides.Internal
{
    internal class VerticalSlide : ISlide, IVerticalSlide, ISlideWithContent
    {
        public int VerticalIndex { get; set; }
        public int HorizontalIndex { get; set; }
        public string Content { get; set; }
    }
}
