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
        private bool _showProgress = true;
        private bool _showSlideNumbers = true;

        [Parameter] public RenderFragment ChildContent { get; set; }

        protected override void OnInitialized()
        {
            SlidesAPI.StateUpdated += StateUpdated;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await _scripts.SetInstance();
                await UpdateJsInteropVars();
                await _scripts.Log("Initial State: ", SlidesAPI.State);
            }
        }

        //Slides API Events
        private void StateUpdated(object sender, State state)
        {
            StateHasChanged();
            _scripts.Log("State: ", state);
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
                StateHasChanged();
            }
        }
    }
}
