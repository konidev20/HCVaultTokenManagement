using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace HashicorpVaultAppRoleAuthentication
{
    internal static class Program
    {
        internal static async Task Main(string[] args) {
            await CreateHostBuilder(args).RunConsoleAsync();
        }

        internal static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
