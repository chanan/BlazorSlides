using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace BlazorSlides.Internal.Components.Themes.White
{
    public partial class Reveal : ComponentBase
    {
        private string _classname;
        private bool _rendered = false;
        [Parameter] public string Classname { get; set; }
        [Parameter] public EventCallback<string> ClassnameChanged { get; set; }

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
