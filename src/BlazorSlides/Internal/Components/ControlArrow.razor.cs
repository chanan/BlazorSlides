using Microsoft.AspNetCore.Components;

namespace BlazorSlides.Internal.Components
{
    public partial class ControlArrow : ComponentBase
    {
        private string _finalArrow;
        private string _controlsArrow;
        private string _arrowRight;
        private string _arrowUp;
        private string _arrowDown;

        [Parameter] public ArrowDirection ArrowDirection { get; set; }
        [Parameter] public bool IsEnabled { get; set; }
    }
}
