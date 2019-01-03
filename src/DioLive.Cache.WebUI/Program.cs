﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace DioLive.Cache.WebUI
{
	public class Program
	{
		public static void Main(string[] args)
		{
			BuildWebHost(args).Run();
		}

		private static IWebHost BuildWebHost(string[] args)
		{
			return WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.Build();
		}
	}
}