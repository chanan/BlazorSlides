using Microsoft.AspNetCore.Components;

namespace BlazorSlides.Internal.Components.Themes.Moon
{
    public partial class Progress : ComponentBase
    {
        [Parameter] public string ProgressClassname { get; set; }
    }
}
