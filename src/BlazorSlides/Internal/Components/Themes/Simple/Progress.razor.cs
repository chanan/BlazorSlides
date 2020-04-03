using Microsoft.AspNetCore.Components;

namespace BlazorSlides.Internal.Components.Themes.Simple
{
    public partial class Progress : ComponentBase
    {
        [Parameter] public string ProgressClassname { get; set; }
    }
}
