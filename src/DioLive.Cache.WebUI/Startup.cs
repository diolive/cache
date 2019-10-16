using System;
using System.Globalization;
using System.IO;

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
		public Startup(IWebHostEnvironment env)
		{
			IConfigurationBuilder builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", true, true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", true);

			builder.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddLocalization(options => options.ResourcesPath = "Resources");

			services.AddControllersWithViews(options => { options.ModelBinderProviders.Insert(0, new DateTimeModelBinderProvider()); })
				.AddViewLocalization()
				.AddDataAnnotationsLocalization();

			// Add application services.

			services.AddTransient<IEmailSender, AuthMessageSender>();
			services.AddTransient<ISmsSender, AuthMessageSender>();
			services.AddSingleton<WordLocalizer>();
			services.AddSingleton(ApplicationOptions.Load());
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			services.AddScoped<ICurrentContext, CurrentContext>();

			services.BindCacheDependencies(Configuration);

			services.Configure<RequestLocalizationOptions>(options =>
			{
				var supportedCultures = new[]
				{
					new CultureInfo(Cultures.English),
					new CultureInfo(Cultures.Russian)
				};

				options.DefaultRequestCulture = new RequestCulture(Cultures.English, Cultures.English);
				options.SupportedCultures = supportedCultures;
				options.SupportedUICultures = supportedCultures;
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
				app.UseHsts();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseStaticFiles();

			if (bool.TryParse(Configuration["EnableHttps"], out bool enableHttps) && enableHttps)
			{
				EnableHttps(app);
			}

			app.UseRouting();

			app.UseCacheAuthentication();
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