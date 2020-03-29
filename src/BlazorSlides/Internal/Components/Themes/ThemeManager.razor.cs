﻿using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace BlazorSlides.Internal.Components.Themes
{
    public partial class ThemeManager : ComponentBase
    {
        private string _classname;
        private bool _rendered = false;

        //Injections
        [CascadingParameter(Name = "SlidesAPI")] public SlidesAPI SlidesAPI { get; set; }

        //Parameters
        [Parameter] public Region Region { get; set; }
        [Parameter] public string Classname { get; set; }
        [Parameter] public EventCallback<string> ClassnameChanged { get; set; }
        [Parameter] public string RevealClassname { get; set; }
        [Parameter] public string ProgressClassname { get; set; }
        [Parameter] public string ControlsClassname { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(_classname != null && !_rendered)
            {
                _rendered = true;
                Classname = _classname;
                await ClassnameChanged.InvokeAsync(Classname);
            }
        }
    }
}
