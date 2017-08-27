using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace DioLive.BlackMint.WebApp
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        private static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();
        }
    }
}