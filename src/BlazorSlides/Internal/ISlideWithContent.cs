using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorSlides.Internal
{
    public interface ISlideWithContent : ISlide
    {
        public string Content { get; set; }
    }
}
