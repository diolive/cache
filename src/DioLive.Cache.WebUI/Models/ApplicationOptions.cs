using System;
using System.IO;

using DioLive.Cache.Storage;

using Microsoft.Extensions.PlatformAbstractions;

namespace DioLive.Cache.WebUI.Models
{
	public class ApplicationOptions
	{
		public string ApplicationVersion { get; set; }

		public DateTime BuildDate { get; set; }

		public string BuildDateString { get; set; }

		public static ApplicationOptions Load()
		{
			ApplicationEnvironment app = PlatformServices.Default.Application;
			DateTime buildDate = File.GetLastWriteTimeUtc(app.ApplicationBasePath);

			return new ApplicationOptions
			{
				BuildDate = buildDate,
				BuildDateString = buildDate.ToString(Constants.DateFormat),
				ApplicationVersion = app.ApplicationVersion.EndsWith(".0")
					? app.ApplicationVersion.Substring(0, app.ApplicationVersion.Length - 2)
					: app.ApplicationVersion
			};
		}
	}
}