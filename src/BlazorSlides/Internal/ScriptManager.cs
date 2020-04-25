using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace BlazorSlides.Internal
{
    internal class ScriptManager
    {
        private IJSRuntime JSRuntime { get; set; }
        private bool _init = false;

        public ScriptManager(IJSRuntime jSRuntime)
        {
            JSRuntime = jSRuntime;
        }

        //Parameters
        public ElementReference? DomWrapper { get; set; }

        private async Task Init()
        {
            try
            {
                if (!_init)
                {
                    await JSRuntime.InvokeVoidAsync("eval", _script);
                    _init = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }

        //Events
        public EventHandler OnWindowResize { get; set; }

        //Javascript commands
        internal async Task<Size> GetScreenSize(double margin, double minScale, double maxScale, int configWidth, int configHeight)
        {
            if (DomWrapper == null)
            {
                return new Size();
            }
            int offsetWidth = await JSRuntime.InvokeAsync<int>("BlazorSlides.offsetWidth", DomWrapper);
            int offsetHeight = await JSRuntime.InvokeAsync<int>("BlazorSlides.offsetHeight", DomWrapper);
            double width = offsetWidth - offsetWidth * margin;
            double height = offsetHeight - offsetHeight * margin;
            double scale = Math.Min(width / configWidth, height / configHeight);
            scale = Math.Max(scale, minScale);
            scale = Math.Min(scale, maxScale);
            return new Size { OffsetHeight = offsetHeight, OffsetWidth = offsetWidth, Height = height, Width = width, Scale = scale };
        }

        internal async Task SetInstance()
        {
            await Init();
            await JSRuntime.InvokeVoidAsync("BlazorSlides.setInstance", DotNetObjectReference.Create(this));
        }

        internal async Task Log(string msg, object obj)
        {
            await Init();
            await JSRuntime.InvokeVoidAsync("BlazorSlides.log", msg, obj);
        }

        internal async Task UpdateHash(string hash)
        {
            await Init();
            await JSRuntime.InvokeVoidAsync("BlazorSlides.updateHash", hash);
        }

        internal async ValueTask<int> GetScrollHeight(ElementReference elementReference)
        {
            await Init();
            return await JSRuntime.InvokeAsync<int>("BlazorSlides.getScrollHeight", elementReference);
        }

        [JSInvokable]
        public void _OnWindowResize()
        {
            OnWindowResize?.Invoke(this, new EventArgs());
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
