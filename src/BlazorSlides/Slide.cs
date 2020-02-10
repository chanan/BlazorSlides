using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorSlides
{
    public partial class Slide
    {
        [Parameter] public string Caption { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }
    }
}
