using Microsoft.AspNetCore.Components;

namespace BlazorSlides.Internal.Components.Fragments
{
    public class FragmentBase : ComponentBase
    {
        //Styles
        protected string _fragmentClass;
        protected string _past;
        protected string _visible;
        protected string _currentFragment;

        //Injections
        [CascadingParameter(Name = "SlidesAPI")] public SlidesAPI SlidesAPI { get; set; }
        [CascadingParameter(Name = "HorizontalIndex")] public int HorizontalIndex { get; set; }
        [CascadingParameter(Name = "VerticalIndex")] public int? VerticalIndex { get; set; }

        //Parameters
        [Parameter] public RenderFragment ChildContent { get; set; }

        public int FragmentIndex { get; private set; }

        protected bool IsPast => FragmentIndex < SlidesAPI.State.CurrentFragmentIndex;
        protected bool IsCurrent => FragmentIndex == SlidesAPI.State.CurrentFragmentIndex;
        protected bool IsVisible => IsPast || IsCurrent;

        //Component events
        protected override void OnInitialized()
        {
            FragmentIndex = SlidesAPI.RegisterFragment(HorizontalIndex, VerticalIndex);
        }
    }
}
