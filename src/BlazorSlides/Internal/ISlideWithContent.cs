namespace BlazorSlides.Internal
{
    public interface ISlideWithContent : ISlide
    {
        public string Content { get; set; }
    }
}
