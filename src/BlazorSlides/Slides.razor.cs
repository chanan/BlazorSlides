using BlazorSlides.Internal.Components;
using Microsoft.AspNetCore.Components;
using Polished;
using System.Threading.Tasks;

namespace BlazorSlides
{
    public partial class Slides : ComponentBase
    {
        //Main reveal container
        private string _revealContainer;
        private string _reveal;
        private string _center;
        private string _revealTheme;
        private string _visible = "visibility: hidden;";

        //Slides container
        private string _slidesContainer;
        private string _slidesClass;
        private string _size;

        private readonly IMixins _mixins = new Mixins();

        //JsInterop
        private Scripts _scripts;
        private ElementReference _domWrapper;

        //State
        private SlidesAPI SlidesAPI { get; } = new SlidesAPI();

        private bool _hasDarkBackground = false;
        private bool _hasLightBackground = true;

        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public ControlsBackArrows ControlsBackArrows { get; set; } = ControlsBackArrows.Faded;
        [Parameter] public ControlsLayout ControlsLayout { get; set; } = ControlsLayout.BottomRight;
        [Parameter] public bool ControlsTutorial { get; set; } = true;
        [Parameter] public bool Controls { get; set; } = true;
        [Parameter] public bool Progress { get; set; } = true;
        [Parameter] public bool SlideNumber { get; set; } = false;
        [Parameter] public SlideNumberFormat SlideNumberFormat { get; set; } = SlideNumberFormat.HorizontalPeriodVertical;
        [Parameter] public int Width { get; set; } = 960;
        [Parameter] public int Height { get; set; } = 700;
        [Parameter] public double Margin { get; set; } = 0.04d;
        [Parameter] public double MinScale { get; set; } = 0.2d;
        [Parameter] public double MaxScale { get; set; } = 2.00d;

        private string HeightInPx => Height + "px";
        private string WidthInPx => Width + "px";

        //Component events
        protected override void OnInitialized()
        {
            SlidesAPI.StateUpdated += StateUpdated;
        }

        protected override void OnParametersSet()
        {
            SlidesAPI.State.ControlsBackArrows = ControlsBackArrows;
            SlidesAPI.State.ControlsLayout = ControlsLayout;
            SlidesAPI.State.ControlsTutorial = ControlsTutorial;
            SlidesAPI.State.Controls = Controls;
            SlidesAPI.State.Progress = Progress;
            SlidesAPI.State.SlideNumber = SlideNumber;
            SlidesAPI.State.SlideNumberFormat = SlideNumberFormat;
            SlidesAPI.State.Width = Width;
            SlidesAPI.State.Height = Height;
            SlidesAPI.State.Margin = Margin;
            SlidesAPI.State.MinScale = MinScale;
            SlidesAPI.State.MaxScale = MaxScale;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await _scripts.SetInstance();
                await UpdateJsInteropVars();
                await _scripts.Log("Initial State: ", SlidesAPI.State);
                SlidesAPI.State.Ready = true;
                _visible = string.Empty;
                await InvokeAsync(() => StateHasChanged());
            }
        }

        //Slides API events
        private async void StateUpdated(object sender, State state)
        {
            await InvokeAsync(StateHasChanged).ConfigureAwait(false);
            await UpdateJsInteropVars();
            await _scripts.Log("State: ", state);
        }

        //Scripts Events
        private async Task _OnWindowResize(object ignored)
        {
            await UpdateJsInteropVars();
        }

        private async Task UpdateJsInteropVars()
        {
            int width = await _scripts.GetOffsetWidth();
            if (width != SlidesAPI.State.SlidesWidth)
            {
                SlidesAPI.State.SlidesWidth = width;
                await InvokeAsync(() => StateHasChanged());
            }
        }
    }
}
