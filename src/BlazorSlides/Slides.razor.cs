using BlazorSlides.Internal;
using BlazorStyled;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Polished;
using System;
using System.Collections.Generic;
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
        [Inject] private IJSRuntime JSRuntime { get; set; }
        private ScriptManager _scriptManager;
        private ElementReference _domWrapper;

        //State
        private SlidesAPI SlidesAPI { get; } = new SlidesAPI();

        private bool _hasDarkBackground = false;
        private bool _hasLightBackground = true;

        //Injections
        [Inject] public NavigationManager NavigationManager { get; set; }

        //Parameters
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public Theme Theme { get; set; } = Theme.Black;
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
        [Parameter] public Transition Transition { get; set; } = Transition.Slide;

        //Component events

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
            SlidesAPI.State.Transition = Transition;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                SlidesAPI.StateUpdated += StateUpdated;
                NavigationManager.LocationChanged += HandleLocationChanged;
                _scriptManager = new ScriptManager(JSRuntime)
                {
                    DomWrapper = _domWrapper
                };
                _scriptManager.OnWindowResize += OnWindowResize;
                await _scriptManager.SetInstance();
                await UpdateJsInteropVars();
                await _scriptManager.Log("Initial State: ", SlidesAPI.State);
                ParseURL(NavigationManager.Uri);
                SlidesAPI.State.Ready = true;
                _visible = string.Empty;
                await UpdateJsInteropVars();
                await InvokeAsync(() => StateHasChanged()).ConfigureAwait(false);
            }
        }

        private void OnKeyPress(KeyboardEventArgs e)
        {
            switch (e.Code)
            {
                case "ArrowRight":
                    SlidesAPI.MoveNext();
                    break;
                case "ArrowLeft":
                    SlidesAPI.MovePrevious();
                    break;
                case "ArrowUp":
                    SlidesAPI.MoveUp();
                    break;
                case "ArrowDown":
                    SlidesAPI.MoveDown();
                    break;
                default:
                    break;
            }
        }

        //Slides API events
        private async void StateUpdated(object sender, State state)
        {
            await UpdateJsInteropVars();
            await _scriptManager.Log("State: ", state);
            string hash = Hash(state);
            await _scriptManager.UpdateHash(hash);
            await UpdateJsInteropVars();
            await InvokeAsync(StateHasChanged).ConfigureAwait(false);
        }

        private string Hash(State state)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("/");
            if (state.CurrentSlideId == null)
            {
                sb.Append(state.CurrentHorizontalIndex);
                if (state.IsVerticalSlide)
                {
                    sb.Append("/");
                    sb.Append(state.CurrentVerticalIndex);
                }
            }
            else
            {
                sb.Append(state.CurrentSlideId);
            }
            return sb.ToString();
        }

        //NavigationManager events
        private void HandleLocationChanged(object sender, LocationChangedEventArgs e)
        {
            ParseURL(e.Location);
        }

        private void ParseURL(string location)
        {
            //Config
            List<KeyValuePair<string, string>> pairs = ParseQuery(location);
            SetConfig(pairs);

            //Navigation
            if (location.Contains("#/"))
            {
                Navigate(location.Substring(location.IndexOf("#/") + 1));
            }
        }

        private void SetConfig(List<KeyValuePair<string, string>> pairs)
        {
            bool changed = false;
            foreach (KeyValuePair<string, string> kvp in pairs)
            {
                switch (kvp.Key.ToLower())
                {
                    case "transition":
                        if (Transition.TryParse<Transition>(kvp.Value, true, out Transition transition))
                        {
                            Transition = transition;
                            SlidesAPI.State.Transition = transition;
                            changed = true;
                        }
                        break;
                    case "theme":
                        if (Theme.TryParse<Theme>(kvp.Value, true, out Theme theme))
                        {
                            Theme = theme;
                            SlidesAPI.State.Theme = theme;
                            changed = true;
                        }
                        break;
                    default:
                        break;
                }
            }
            if (changed)
            {
                SlidesAPI.UpdateStatus();
            }
        }

        private List<KeyValuePair<string, string>> ParseQuery(string location)
        {
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
            if (location.IndexOf("?") == -1)
            {
                return list;
            }

            int start = location.IndexOf("?") + 1;
            int end = location.IndexOf("#") != -1 ? location.IndexOf("#") : location.Length;
            string query = location.Substring(start, end - start);
            string[] arr = query.Split("&");
            foreach (string strPair in arr)
            {
                string[] arrPair = strPair.Split("=");
                KeyValuePair<string, string> pair = new KeyValuePair<string, string>(arrPair[0], arrPair[1]);
                list.Add(pair);
            }
            return list;
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
            if (arr.Length != 3)
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
            if (int.TryParse(arr[1], out int result))
            {
                return result;
            }
            else
            {
                if (SlidesAPI.State.TryGetHorizontalById(arr[1], out int res))
                {
                    return res;
                }
                else
                {
                    return 0;
                }
            }
        }

        //Scripts Events
        private async void OnWindowResize(object sender, EventArgs e)
        {
            await UpdateJsInteropVars();
            SlidesAPI.UpdateStatus();
        }

        private async Task UpdateJsInteropVars()
        {
            foreach (InternalSlide slide in SlidesAPI.State.Slides)
            {
                int scrollHeight = await _scriptManager.GetScrollHeight(slide.ElementReference);
                slide.Top = Math.Max((Height - scrollHeight) / 2, 0);
            }

            Size size = await _scriptManager.GetScreenSize(Margin, SlidesAPI.State.MinScale, SlidesAPI.State.MaxScale, Width, Height);
            if (size.Width != SlidesAPI.State.ComputedSize.Width || size.Height != SlidesAPI.State.ComputedSize.Height)
            {
                SlidesAPI.State.ComputedSize = size;
                await InvokeAsync(StateHasChanged).ConfigureAwait(false);
            }
        }

        //Private methods
        private string DoubleToPx(double value)
        {
            return DoubleToString(value) + "px";
        }

        private string DoubleToString(double value)
        {
            return value.ToString("F");
        }
    }
}
