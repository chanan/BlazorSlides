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

        //Javascript commands
        internal async Task<Size> GetScreenSize()
        {
            if (DomWrapper == null)
            {
                return new Size();
            }
            int width = await JSRuntime.InvokeAsync<int>("BlazorSlides.offsetWidth", DomWrapper);
            int height = await JSRuntime.InvokeAsync<int>("BlazorSlides.offsetHeight", DomWrapper);
            return new Size { Height = height, Width = width };
        }

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

        internal async Task UpdateHash(string hash)
        {
            await JSRuntime.InvokeVoidAsync("BlazorSlides.updateHash", hash);
        }

        internal ValueTask<int> GetScrollHeight(ElementReference elementReference)
        {
            return JSRuntime.InvokeAsync<int>("BlazorSlides.getScrollHeight", elementReference);
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
    offsetHeight: function (domWrapper) {
        return domWrapper ? domWrapper.offsetHeight : 0;
    },
    log: function (msg, obj) {
        console.log(msg, obj);
    },
    updateHash: function(hash) {
        if(window.location.hash !== hash) {
            window.location.hash = hash;
        }
    },
    getScrollHeight: function (element) {
        return element ? element.scrollHeight : 0;
    }
}

function onWindowResize() {
    instance.invokeMethodAsync('_OnWindowResize');
}
";
    }
}
