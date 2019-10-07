using System;
using System.IO;

using DioLive.Cache.Storage;

using Microsoft.Extensions.PlatformAbstractions;

namespace DioLive.Cache.WebUI.Models
{
	public class ApplicationOptions
	{
		public ApplicationOptions(ApplicationEnvironment app)
		{
			string applicationVersion = app.ApplicationVersion;
			DateTime buildDate = File.GetLastWriteTimeUtc(app.ApplicationBasePath);

			BuildDate = buildDate;
			BuildDateString = buildDate.ToString(Constants.DateFormat);
			ApplicationVersion = applicationVersion.EndsWith(".0")
				? applicationVersion[..^2]
				: applicationVersion;
		}

		public string ApplicationVersion { get; }

		public DateTime BuildDate { get; }

		public string BuildDateString { get; }

		public static ApplicationOptions Load()
		{
			return new ApplicationOptions(PlatformServices.Default.Application);
		}
	}
}