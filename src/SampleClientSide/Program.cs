using BlazorStyled;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SampleCore;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SampleClientSide
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);

            //Configure Services
            //IConfigurationSection section = builder.Configuration.Build().GetSection("BlazorStyled");
            //builder.Services.AddBlazorStyled(isDevelopment: GetValue(section, "development"), isDebug: GetValue(section, "debug"));

            builder.Services.AddBlazorStyled(isDevelopment: false, isDebug: true);
            //End Configure Services

            builder.RootComponents.Add<App>("app");

            await builder.Build().RunAsync();
        }

        private static bool GetValue(IConfigurationSection section, string key)
        {
            if(section == null)
            {
                return false;
            }
            if(bool.TryParse(section[key], out bool result))
            {
                return result;
            }
            return false;
        }
    }
}
