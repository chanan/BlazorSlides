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
        private string _hideSlide;
        private string _showSlide;
        private string _currentSlideClass;
        private string _captionClass;
        private string _slideNumberClass;
        private string _fade;

        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public int NumberOfSlides { get; set; }
        [Parameter] public int CurrentSlide { get; set; }
        [Parameter] public string Caption { get; set; }
        [Parameter] public int Index { get; set; }

        protected override void OnParametersSet()
        {
            _currentSlideClass = Index == CurrentSlide ? _showSlide : _hideSlide;
        }
    }
}
