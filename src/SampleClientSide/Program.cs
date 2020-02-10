using BlazorStyled;
using Microsoft.AspNetCore.Blazor.Hosting;
using SampleCore;
using System.Threading.Tasks;

namespace SampleClientSide
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            //Configure Services
            builder.Services.AddBlazorStyled();
            //End Configure Services

            builder.RootComponents.Add<App>("app");

            //Add BlazorStyled to root components
            builder.RootComponents.Add<ClientSideStyled>("#styled");

            await builder.Build().RunAsync();
        }
    }
}
