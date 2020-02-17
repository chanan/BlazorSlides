using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorSlides.Internal
{
    public interface ISlideWithContent
    {
        public string Content { get; set; }
    }
}
