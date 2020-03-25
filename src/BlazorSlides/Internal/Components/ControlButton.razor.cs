using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorSlides.Internal.Components
{
    public partial class ControlButton : ComponentBase
    {
        private string _button;
        private string _enabled;
        private string _navigateLeft;
        private string _navigateRight;
        private string _navigateUp;
        private string _navigateDown;
        private string _locationOverrideLeft;
        private string _locationOverrideRight;
        private string _locationOverrideUp;
        private string _locationOverrideDown;

        [Parameter] public ArrowDirection ArrowDirection { get; set; }
        [Parameter] public bool IsEnabled { get; set; }
        [Parameter] public bool HasHorizontal { get; set; }
        [Parameter] public bool HasVertical { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
    }
}
