using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace BlazorSlides.Internal.Components.Themes.Sky
{
    public partial class SkyTheme : ComponentBase
    {
        private string _classname;
        private bool _rendered = false;
        [Parameter] public Theme Theme { get; set; }
        [Parameter] public Region Region { get; set; }
        [Parameter] public string Classname { get; set; }
        [Parameter] public EventCallback<string> ClassnameChanged { get; set; }
        [Parameter] public string RevealClassname { get; set; }
        [Parameter] public string ProgressClassname { get; set; }
        [Parameter] public string ControlsClassname { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (_classname != null && !_rendered)
            {
                _rendered = true;
                Classname = _classname;
                await ClassnameChanged.InvokeAsync(Classname);
            }
        }
    }
}
