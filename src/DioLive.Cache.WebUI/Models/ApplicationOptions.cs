using Microsoft.Extensions.PlatformAbstractions;

namespace DioLive.Cache.WebUI.Models
{
	public class ApplicationOptions
	{
		public ApplicationOptions(ApplicationEnvironment app)
		{
			ApplicationVersion = app.ApplicationVersion;
		}

		public string ApplicationVersion { get; }

		public static ApplicationOptions Load()
		{
			return new ApplicationOptions(PlatformServices.Default.Application);
		}
	}
}