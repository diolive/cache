using System;
using System.IO;

using Microsoft.Extensions.PlatformAbstractions;

namespace DioLive.Cache.WebUI.Models
{
    public class ApplicationOptions
    {
        public static ApplicationOptions Load()
        {
            var app = PlatformServices.Default.Application;
            DateTime buildDate = File.GetLastWriteTimeUtc(app.ApplicationBasePath);

            return new ApplicationOptions
            {
                ApplicationVersion = app.ApplicationVersion,
                BuildDate = buildDate,
                BuildDateString = buildDate.ToString(Binders.DateTimeModelBinder.DateFormat),
            };
        }

        public string ApplicationVersion { get; set; }

        public DateTime BuildDate { get; set; }

        public string BuildDateString { get; set; }
    }
}