using Microsoft.AspNetCore.Components;

namespace BlazorSlides
{
    public partial class Fragment : ComponentBase
    {
        //Parameters
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public FragmentStyle FragmentStyle { get; set; } = FragmentStyle.Default;
    }
}
