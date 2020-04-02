﻿using Microsoft.AspNetCore.Components;

namespace BlazorSlides.Internal.Components.Transitions
{
    public class TransitionBase : ComponentBase
    {
        //Injections
        [CascadingParameter(Name = "SlidesAPI")] public SlidesAPI SlidesAPI { get; set; }

        //Parameters
        [Parameter] public string SectionClassname { get; set; }
        [Parameter] public string PastSectionClassname { get; set; }
        [Parameter] public string FutureSectionClassname { get; set; }
        [Parameter] public string StackClassname { get; set; }
        [Parameter] public string VerticalSectionClassname { get; set; }
        [Parameter] public string VerticalPastSectionClassname { get; set; }
        [Parameter] public string VerticalFutureSectionClassname { get; set; }
    }
}