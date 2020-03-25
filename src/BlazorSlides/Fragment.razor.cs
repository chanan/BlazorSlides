using Microsoft.AspNetCore.Components;

namespace BlazorSlides
{
    public partial class Fragment : ComponentBase
    {
        //Styles
        private string _fragmentClass;
        private string _fragment;
        private string _visible;
        private string _currentFragment;

        //Injections
        [CascadingParameter(Name = "SlidesAPI")] public SlidesAPI SlidesAPI { get; set; }
        [CascadingParameter(Name = "HorizontalIndex")] public int? HorizontalIndex { get; set; }
        [CascadingParameter(Name = "VerticalIndex")] public int? VerticalIndex { get; set; }

        //Parameters
        [Parameter] public RenderFragment ChildContent { get; set; }

        private bool IsPast()
        {
            return false;
        }

        private bool IsCurrent()
        {
            return false;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }
    }
}
