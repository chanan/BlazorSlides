using Microsoft.AspNetCore.Components;

namespace BlazorSlides.Internal.Components
{
    public partial class Controls : ComponentBase
    {
        //Styles
        private string _controlsFinal;
        private string _controls;
        private string _dark;
        private string _light;

        //Injections
        [CascadingParameter(Name = "SlidesAPI")] public SlidesAPI SlidesAPI { get; set; }

        //Parameters
        [Parameter] public bool HasDarkBackground { get; set; }
        [Parameter] public bool HasLightBackground { get; set; }

        //Components events
        private void OnNext()
        {
            SlidesAPI.MoveNext();
        }

        private void OnPrevious()
        {
            SlidesAPI.MovePrevious();
        }

        private void OnDown()
        {
            SlidesAPI.MoveDown();
        }

        private void OnUp()
        {
            SlidesAPI.MoveUp();
        }

        //Private properties
        private bool IsArrowLeftEnabled => SlidesAPI.State.HasPreviousSlide || (!SlidesAPI.State.IsVerticalSlide && SlidesAPI.State.HasPreviousFragment);
        private bool IsArrowRightEnabled => SlidesAPI.State.HasNextSlide || (!SlidesAPI.State.IsVerticalSlide && SlidesAPI.State.HasNextFragment);
        private bool IsArrowUpEnabled => SlidesAPI.State.HasUpSlide || (SlidesAPI.State.IsVerticalSlide && SlidesAPI.State.HasPreviousFragment);
        private bool IsArrowDownEnabled => SlidesAPI.State.HasDownSlide || (SlidesAPI.State.IsVerticalSlide && SlidesAPI.State.HasNextFragment);

        /*TODO:
        * navigation mode
        * layout and media query
        * no hover(used on mobile)
        * move location if numbers?*/
    }
}
