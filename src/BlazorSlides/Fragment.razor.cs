using Microsoft.AspNetCore.Components;

namespace BlazorSlides
{
    public partial class Fragment : ComponentBase
    {
        //Styles
        private string _fragmentClass;
        private string _past;
        private string _visible;
        private string _currentFragment;

        //Injections
        [CascadingParameter(Name = "SlidesAPI")] public SlidesAPI SlidesAPI { get; set; }
        [CascadingParameter(Name = "HorizontalIndex")] public int HorizontalIndex { get; set; }
        [CascadingParameter(Name = "VerticalIndex")] public int? VerticalIndex { get; set; }

        //Parameters
        [Parameter] public RenderFragment ChildContent { get; set; }
        public int FragmentIndex { get; private set; }

        private bool IsPast => FragmentIndex < SlidesAPI.State.CurrentFragmentIndex;
        private bool IsCurrent => FragmentIndex == SlidesAPI.State.CurrentFragmentIndex;
        private bool IsVisible => IsPast || IsCurrent;

        //Component events
        protected override void OnInitialized()
        {
            FragmentIndex = SlidesAPI.RegisterFragment(HorizontalIndex, VerticalIndex);
        }
    }
}
