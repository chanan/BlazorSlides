using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorSlides.Internal
{
    public partial class InternalSlide
    {
        //Css classes
        private string _currentSlideClass;
        private string _slide;
        private string _center;
        private string _notPresent;
        private string _future;
        private string _present;
        private string _past;

        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public int NumberOfSlides { get; set; }
        [Parameter] public int CurrentSlide { get; set; }
        [Parameter] public string Caption { get; set; }
        [Parameter] public int Index { get; set; }
    }
}
