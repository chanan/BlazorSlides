using System.Collections.Generic;

namespace BlazorSlides.Internal
{
    public interface ISlideWithContent : ISlide
    {
        public IEnumerable<IContent> Contents { get; set; }
        public bool HasFragment();
        public int FragmentCount();
        public int CurrentFragment { get; }
        public bool NextFragment();
        public bool PreviousFragment();
    }
}
