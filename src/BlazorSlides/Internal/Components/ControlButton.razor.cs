using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorSlides.Internal.Components
{
    public partial class ControlButton : ComponentBase
    {
        //Styles
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
        private string _faded;
        private string _fragmented;
        private string _hidden;
        private string _bounceRight;
        private string _bounceDown;
        private string _highlightRight;
        private string _highlightDown;

        private bool _hasClicked = false;

        //Injections
        [CascadingParameter(Name = "SlidesAPI")] public SlidesAPI SlidesAPI { get; set; }

        //Parameters
        [Parameter] public ArrowDirection ArrowDirection { get; set; }
        [Parameter] public bool IsEnabled { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

        //Private parameters
        private bool IsFaded => SlidesAPI.State.ControlsBackArrows == ControlsBackArrows.Faded && (ArrowDirection == ArrowDirection.Up || ArrowDirection == ArrowDirection.Left);
        private bool IsFragmented => SlidesAPI.State.HasPreviousFragment || SlidesAPI.State.HasNextFragment;
        private bool ShouldHighlight => SlidesAPI.State.ControlsTutorial && (ArrowDirection == ArrowDirection.Right || ArrowDirection == ArrowDirection.Down) && !_hasClicked;

        //Events
        private void _onClick(MouseEventArgs e)
        {
            _hasClicked = true;
            if (OnClick.HasDelegate)
            {
                OnClick.InvokeAsync(e);
            }
        }
    }
}
