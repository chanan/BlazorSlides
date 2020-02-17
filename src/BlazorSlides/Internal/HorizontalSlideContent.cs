namespace BlazorSlides.Internal
{
    internal class HorizontalSlideContent : IHorizontalSlide, ISlideWithContent
    {
        public int HorizontalIndex { get; set; }
        public string Content { get; set; }
    }
}
