﻿using BlazorSlides.Internal.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Polished;
using System;
using System.Text;
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

        //Injections
        [Inject] public NavigationManager NavigationManager { get; set; }

        //Parameters
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public Theme Theme { get; set; } = Theme.White;
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
            NavigationManager.LocationChanged += HandleLocationChanged;
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
            SlidesAPI.State.Theme = Theme;
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
            await UpdateJsInteropVars();
            await _scripts.Log("State: ", state);
            string hash = Hash(state);
            await _scripts.UpdateHash(hash);
            await InvokeAsync(StateHasChanged).ConfigureAwait(false);
        }

        private string Hash(State state)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("/");
            sb.Append(state.CurrentHorizontalIndex);
            if(state.IsVerticalSlide)
            {
                sb.Append("/");
                sb.Append(state.CurrentVerticalIndex);
            }
            return sb.ToString();
        }

        //NavigationManager events
        private void HandleLocationChanged(object sender, LocationChangedEventArgs e)
        {
            if (e.Location.Contains("#/"))
            {
                Navigate(e.Location.Substring(e.Location.IndexOf("#/") + 1));
            }
        }

        private void Navigate(string hash)
        {
            int horizontal = ParseHorizontal(hash);
            int? vertical = ParseVertical(hash);
            SlidesAPI.NavigateTo(horizontal, vertical);
        }

        private int? ParseVertical(string hash)
        {
            string[] arr = hash.Split('/');
            if(arr.Length != 3)
            {
                return null;
            }
            else
            {
                if (int.TryParse(arr[2], out int result))
                {
                    return result;
                }
                else
                {
                    return null;
                }
            }
        }

        private int ParseHorizontal(string hash)
        {
            string[] arr = hash.Split('/');
            if(int.TryParse(arr[1], out int result))
            {
                return result;
            }
            else
            {
                //This an id
                throw new NotImplementedException();
            }
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
