using System.IO;

using Microsoft.AspNetCore.Hosting;

namespace DioLive.BlackMint.WebApp
{
    public static class Program
    {
        public static void Main()
        {
            IWebHost host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();
        }
    }
}