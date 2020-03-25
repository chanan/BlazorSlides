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

        public async void OnNext()
        {
            await SlidesAPI.MoveNext();
        }

        public async void OnPrevious()
        {
            await SlidesAPI.MovePrevious();
        }

        public async void OnDown()
        {
            await SlidesAPI.MoveDown();
        }

        public async void OnUp()
        {
            await SlidesAPI.MoveUp();
        }

        private bool IsArrowLeftEnabled => SlidesAPI.State.CurrentHorizontalIndex > 0;
        private bool IsArrowRightEnabled => SlidesAPI.State.CurrentHorizontalIndex < SlidesAPI.State.HorizontalSlideCount - 1;
        private bool IsArrowUpEnabled => SlidesAPI.State.VerticalSlideCount != 0 && SlidesAPI.State.CurrentVerticalIndex > 0;
        private bool IsArrowDownEnabled => SlidesAPI.State.VerticalSlideCount != 0 && SlidesAPI.State.CurrentVerticalIndex < SlidesAPI.State.VerticalSlideCount - 1;

        /*TODO:
        * Highlight
        * navigation mode
        * fragments
        * layout and media query
        * no hover(used on mobile)
        * move location if numbers?*/
    }
}
