using BlazorStyled;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SampleCore;
using System.Threading.Tasks;

namespace SampleClientSide
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);

            //Configure Services
            builder.Services.AddBlazorStyled(isDevelopment: true, isDebug: true);
            //End Configure Services

            builder.RootComponents.Add<App>("app");

            //Add BlazorStyled to root components

            await builder.Build().RunAsync();
        }
    }
}
