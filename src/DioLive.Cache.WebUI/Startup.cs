using System;
using System.Globalization;
using System.IO;
using System.Linq;

using DioLive.Cache.Binder;
using DioLive.Cache.Common;
using DioLive.Cache.Common.Localization;
using DioLive.Cache.WebUI.Binders;
using DioLive.Cache.WebUI.Models;
using DioLive.Cache.WebUI.Services;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DioLive.Cache.WebUI
{
	public class Startup
	{
		private readonly IConfigurationRoot _configuration;

		public Startup(IWebHostEnvironment env)
		{
			IConfigurationBuilder builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", true, true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
				.AddEnvironmentVariables();

			_configuration = builder.Build();
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddLocalization(options => options.ResourcesPath = "Resources");

			services.AddControllersWithViews(options =>
			{
				options.ModelBinderProviders.Insert(0, new DateTimeModelBinderProvider());
				options.ModelBinderProviders.Insert(1, new DecimalModelBinderProvider());
			})
				.AddViewLocalization()
				.AddDataAnnotationsLocalization();

			// Add application services.

			services.AddTransient<IEmailSender, AuthMessageSender>();
			services.AddTransient<ISmsSender, AuthMessageSender>();
			services.AddSingleton<WordLocalizer>();
			services.AddSingleton(ApplicationOptions.Load());
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			services.AddScoped<ICurrentContext, HttpCurrentContext>();

			services.BindCacheDependencies(_configuration);

			services.Configure<RequestLocalizationOptions>(options =>
			{
				var supportedCulturesNames = _configuration.GetSection("SupportedCultures").Get<string[]>();
				CultureInfo[] supportedCultures = supportedCulturesNames
					.Select(name => new CultureInfo(name))
					.ToArray();
				CultureInfo defaultCulture = supportedCultures.First();

				options.DefaultRequestCulture = new RequestCulture(defaultCulture, defaultCulture);
				options.SupportedCultures = supportedCultures;
				options.SupportedUICultures = supportedCultures;

				Cultures.Default = defaultCulture.Name;
			});

			services.AddDistributedMemoryCache();
			services.AddSession(options => { options.IdleTimeout = TimeSpan.FromDays(1); });
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
			app.UseRequestLocalization(locOptions.Value);

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
				app.UseBrowserLink();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseHsts();
			app.UseStaticFiles();

			if (bool.TryParse(_configuration["EnableHttps"], out bool enableHttps) && enableHttps)
			{
				EnableHttps(app);
			}

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseSession();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					"default",
					"{controller=Home}/{action=Index}/{id?}");
			});
		}

		private static void EnableHttps(IApplicationBuilder app)
		{
			// Let's encrypt
			string root = Path.Combine(Directory.GetCurrentDirectory(), @".well-known");
			if (!Directory.Exists(root))
			{
				Directory.CreateDirectory(root);
			}

			app.UseStaticFiles(new StaticFileOptions
			{
				FileProvider = new PhysicalFileProvider(root),
				RequestPath = new PathString(@"/.well-known"),
				ServeUnknownFileTypes = true // serve extensionless file
			});

			app.UseRewriter(new RewriteOptions()
				.AddRedirectToHttps());
		}
	}
}