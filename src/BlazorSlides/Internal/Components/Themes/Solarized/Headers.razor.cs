using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace BlazorSlides.Internal.Components.Themes.Solarized
{
    public partial class Headers : ComponentBase
    {
        [Parameter] public string RevealClassname { get; set; }
    }
}
