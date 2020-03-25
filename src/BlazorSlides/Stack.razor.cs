using BlazorStyled;
using Microsoft.AspNetCore.Components;

namespace BlazorSlides
{
    public partial class Stack : ComponentBase
    {
        //Styles
        private string _stack;
        private string _currentSlideClass;
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

        public int HorizontalIndex { get; private set; }
        public bool IsPresent { get => HorizontalIndex == SlidesAPI.State.CurrentHorizontalIndex; }
        public bool IsPast { get => HorizontalIndex < SlidesAPI.State.CurrentHorizontalIndex; }
        public bool IsFuture { get => HorizontalIndex > SlidesAPI.State.CurrentHorizontalIndex; }

        protected override void OnInitialized()
        {
            HorizontalIndex = SlidesAPI.ResgisterStack();
        }
    }
}
