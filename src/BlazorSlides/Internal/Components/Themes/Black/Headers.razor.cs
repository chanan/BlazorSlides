using Microsoft.AspNetCore.Components;
using System;

namespace BlazorSlides.Internal.Components.Themes.Black
{
    public partial class Headers : ComponentBase
    {
        [Parameter] public string RevealClassname { get; set; }

        protected override void OnParametersSet()
        {
            Console.WriteLine(RevealClassname);
        }
    }
}
