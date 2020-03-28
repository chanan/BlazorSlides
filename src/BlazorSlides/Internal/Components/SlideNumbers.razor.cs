using Microsoft.AspNetCore.Components;

namespace BlazorSlides.Internal.Components
{
    public partial class SlideNumbers : ComponentBase
    {
        //Styles
        private string _slideNumber;
        private string _delimiter;

        //Injections
        [CascadingParameter(Name = "SlidesAPI")] public SlidesAPI SlidesAPI { get; set; }

        private string GetLink()
        {
            return SlidesAPI.State.VerticalSlideCount == 0 ? "#/" + SlidesAPI.State.CurrentHorizontalIndex : "#/" + SlidesAPI.State.CurrentHorizontalIndex + "/" + SlidesAPI.State.CurrentVerticalIndex;
        }

        private int CurrentHorizontalNumber => SlidesAPI.State.CurrentHorizontalIndex + 1;
        private int CurrentVerticalNumber => SlidesAPI.State.CurrentVerticalIndex + 1;
        /* TODO:
        * Fragments
        * Delimiter */
    }
}
