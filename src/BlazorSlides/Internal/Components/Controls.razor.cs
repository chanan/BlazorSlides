﻿using Microsoft.AspNetCore.Components;

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

        public void OnNext()
        {
            SlidesAPI.MoveNext();
        }

        public void OnPrevious()
        {
            SlidesAPI.MovePrevious();
        }

        public void OnDown()
        {
            SlidesAPI.MoveDown();
        }

        public void OnUp()
        {
            SlidesAPI.MoveUp();
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
