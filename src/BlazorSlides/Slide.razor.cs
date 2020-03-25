using BlazorStyled;
using Microsoft.AspNetCore.Components;

namespace BlazorSlides
{
    public partial class Slide : ComponentBase
    {
        //Styles
        private string _slide;
        private string _notPresent;
        private string _future;
        private string _present;
        private string _past;

        //Parameters
        [Parameter] public RenderFragment ChildContent { get; set; }

        //Injections
        [Inject] IStyled IStyled { get; set; }
        [CascadingParameter(Name = "SlidesAPI")] public SlidesAPI SlidesAPI { get; set; }
        [CascadingParameter(Name = "ParentIndex")] public int? ParentIndex { get; set; }

        public int HorizontalIndex { get; private set; }
        public int? VerticalIndex { get; private set; }
        public bool IsPresent {
            get => (!VerticalIndex.HasValue && 
                        HorizontalIndex == SlidesAPI.State.CurrentHorizontalIndex) ||
                   (VerticalIndex.HasValue && 
                        HorizontalIndex == SlidesAPI.State.CurrentHorizontalIndex &&
                        VerticalIndex.Value == SlidesAPI.State.CurrentVerticalIndex);
        }
        public bool IsPast { 
            get =>  (!VerticalIndex.HasValue &&
                        HorizontalIndex < SlidesAPI.State.CurrentHorizontalIndex) || 
                    (VerticalIndex.HasValue && 
                        HorizontalIndex == SlidesAPI.State.CurrentHorizontalIndex && 
                        VerticalIndex.Value < SlidesAPI.State.CurrentVerticalIndex); 
        }
        public bool IsFuture { 
            get =>  (!VerticalIndex.HasValue && 
                        HorizontalIndex > SlidesAPI.State.CurrentHorizontalIndex) || 
                    (VerticalIndex.HasValue && 
                        HorizontalIndex == SlidesAPI.State.CurrentHorizontalIndex && 
                    VerticalIndex.Value > SlidesAPI.State.CurrentVerticalIndex); 
        }
        public bool IsVertical { get; private set; }

        protected override void OnInitialized()
        {
            if(!ParentIndex.HasValue)
            {
                HorizontalIndex = SlidesAPI.ResgisterHorizontalSlide();
            }
            else
            {
                HorizontalIndex = ParentIndex.Value;
                VerticalIndex = SlidesAPI.ResgisterVerticalSlide(HorizontalIndex);
                IsVertical = true;
            }
        }
    }
}
