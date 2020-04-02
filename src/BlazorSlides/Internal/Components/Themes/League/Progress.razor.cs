using Microsoft.AspNetCore.Components;

namespace BlazorSlides.Internal.Components.Themes.League
{
    public partial class Progress : ComponentBase
    {
        [Parameter] public string ProgressClassname { get; set; }
    }
}
