using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace BlazorSlides.Internal.Components
{
    partial class Scripts : ComponentBase
    {
        private bool _shouldRender = true;

        [Inject] IJSRuntime JSRuntime { get; set; }

        //Parameters
        [Parameter] public ElementReference? DomWrapper { get; set; }

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

        internal async Task SetInstance()
        {
            await JSRuntime.InvokeVoidAsync("BlazorSlides.setInstance", DotNetObjectReference.Create(this));
        }

        [JSInvokable]
        public async void _OnWindowResize()
        {
            await OnWindowResize.InvokeAsync(new object());
        }

        protected override void OnAfterRender(bool firstRender)
        {
            _shouldRender = false;
        }

        protected override bool ShouldRender()
        {
            return _shouldRender;
        }

        private string _script = @"
<script>
    let instance;
    window.BlazorSlides = {
        setInstance: function (_instance) {
            instance = _instance;
            window.addEventListener('resize', onWindowResize, false);
        },
        offsetWidth: function (domWrapper) {
            return domWrapper ? domWrapper.offsetWidth : 0;
        }
    }

    function onWindowResize() {
        instance.invokeMethodAsync('_OnWindowResize');
    }
</script>
";
    }
}
