using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorSlides.Internal.Components
{
    public class Scripts : ComponentBase
    {
        private SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        //Injections
        [Inject] private IJSRuntime JSRuntime { get; set; }

        //Parameters
        [Parameter] public ElementReference? DomWrapper { get; set; }

        //Components
        protected override async Task OnInitializedAsync()
        {
            await _lock.WaitAsync();
        }

        protected override async void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("eval", _script);
                _lock.Release();
            }
        }

        //Events
        [Parameter] public EventCallback OnWindowResize { get; set; }

        internal ValueTask<int> GetOffsetWidth()
        {
            if (DomWrapper == null)
            {
                return new ValueTask<int>(0);
            }
            return JSRuntime.InvokeAsync<int>("BlazorSlides.offsetWidth", DomWrapper);
        }

        //Javascript commands
        internal async Task SetInstance()
        {
            await _lock.WaitAsync();
            await JSRuntime.InvokeVoidAsync("BlazorSlides.setInstance", DotNetObjectReference.Create(this));
            _lock.Release();
        }

        internal async Task Log(string msg, object obj)
        {
            await JSRuntime.InvokeVoidAsync("BlazorSlides.log", msg, obj);
        }

        [JSInvokable]
        public async void _OnWindowResize()
        {
            await OnWindowResize.InvokeAsync(new object());
        }

        private readonly string _script = @"
let instance;
window.BlazorSlides = {
    setInstance: function (_instance) {
        instance = _instance;
        window.addEventListener('resize', onWindowResize, false);
    },
    offsetWidth: function (domWrapper) {
        return domWrapper ? domWrapper.offsetWidth : 0;
    },
    log: function (msg, obj) {
        console.log(msg, obj);
    }
}

function onWindowResize() {
    instance.invokeMethodAsync('_OnWindowResize');
}
";
    }
}
