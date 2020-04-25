using BlazorStyled;
using Microsoft.AspNetCore.Components;

namespace BlazorSlides
{
    public partial class Slide : ComponentBase
    {
        //Styles
        private string _slide;
        private string _section;
        private string _notPresent;
        private string _future;
        private string _present;
        private string _past;
        private string _center;

        private ElementReference _domSlide;

        //Injections
        [Inject] private IStyled IStyled { get; set; }
        [CascadingParameter(Name = "SlidesAPI")] public SlidesAPI SlidesAPI { get; set; }
        [CascadingParameter(Name = "ParentIndex")] public int? ParentIndex { get; set; }

        //Parameters
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public string Id { get; set; }
        public int HorizontalIndex { get; private set; }
        public int? VerticalIndex { get; private set; }
        public string Label => IsVertical ? "vertical-section" : "section";
        public bool IsPresent => (!VerticalIndex.HasValue &&
                                    HorizontalIndex == SlidesAPI.State.CurrentHorizontalIndex) ||
                                 (VerticalIndex.HasValue &&
                                    HorizontalIndex == SlidesAPI.State.CurrentHorizontalIndex &&
                                    VerticalIndex.Value == SlidesAPI.State.CurrentVerticalIndex);
        public bool IsPast => (!VerticalIndex.HasValue &&
                                HorizontalIndex < SlidesAPI.State.CurrentHorizontalIndex) ||
                              (VerticalIndex.HasValue &&
                                HorizontalIndex == SlidesAPI.State.CurrentHorizontalIndex &&
                                VerticalIndex.Value < SlidesAPI.State.CurrentVerticalIndex);

        public bool IsFuture => (!VerticalIndex.HasValue &&
                                    HorizontalIndex > SlidesAPI.State.CurrentHorizontalIndex) ||
                                (VerticalIndex.HasValue &&
                                    HorizontalIndex == SlidesAPI.State.CurrentHorizontalIndex &&
                                    VerticalIndex.Value > SlidesAPI.State.CurrentVerticalIndex);

        public bool IsVertical { get; private set; }

        public string Top => SlidesAPI.State.GetSlide(HorizontalIndex, VerticalIndex).Top + "px";

        protected override void OnInitialized()
        {
            if (!ParentIndex.HasValue)
            {
                HorizontalIndex = SlidesAPI.ResgisterHorizontalSlide(Id);
            }
            else
            {
                HorizontalIndex = ParentIndex.Value;
                VerticalIndex = SlidesAPI.ResgisterVerticalSlide(HorizontalIndex, Id);
                IsVertical = true;
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
           if(firstRender)
            {
                SlidesAPI.UpdateSlideElementReference(HorizontalIndex, VerticalIndex, _domSlide);
            }
        }
    }
}
