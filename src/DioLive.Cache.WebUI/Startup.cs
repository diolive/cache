﻿using System;
using System.Globalization;
using System.IO;

using AutoMapper;

using DioLive.Cache.Models;
using DioLive.Cache.Models.Data;
using DioLive.Cache.Storage;
using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.WebUI.Binders;
using DioLive.Cache.WebUI.Models;
using DioLive.Cache.WebUI.Models.CategoryViewModels;
using DioLive.Cache.WebUI.Models.PlanViewModels;
using DioLive.Cache.WebUI.Models.PurchaseViewModels;
using DioLive.Cache.WebUI.Services;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace DioLive.Cache.WebUI
{
	public class Startup
	{
		public Startup(IHostingEnvironment env)
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
			// Add framework services.
			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("DioLive.Cache.Models")));

			services.AddIdentity<ApplicationUser, IdentityRole>(options =>
				{
					options.Password.RequiredLength = 6;
					options.Password.RequireDigit = false;
					options.Password.RequireLowercase = false;
					options.Password.RequireNonAlphanumeric = false;
					options.Password.RequireUppercase = false;
				})
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();

			services.AddLocalization(options => options.ResourcesPath = "Resources");

			services.AddMvc(options => { options.ModelBinderProviders.Insert(0, new DateTimeModelBinderProvider()); })
				.AddViewLocalization()
				.AddDataAnnotationsLocalization();

			// Add application services.
			services.AddTransient<IEmailSender, AuthMessageSender>();
			services.AddTransient<ISmsSender, AuthMessageSender>();
			services.AddSingleton(Localization.PurchasesPluralizer);
			services.AddSingleton(ApplicationOptions.Load());
			services.AddSingleton<IMapper>(new Mapper(CreateMapperConfiguration()));
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddTransient<CurrentContext>();
			services.AddTransient<ICurrentContext, CurrentContext>();
			services.AddTransient<IApplicationUsersStorage, ApplicationUsersStorage>();
			services.AddTransient<IBudgetsStorage, BudgetsStorage>();
			services.AddTransient<ICategoriesStorage, CategoriesStorage>();
			services.AddTransient<IPlansStorage, PlansStorage>();
			services.AddTransient<IPurchasesStorage, PurchasesStorage>();

			services.Configure<RequestLocalizationOptions>(options =>
			{
				var supportedCultures = new[]
				{
					new CultureInfo("en-US"),
					new CultureInfo("ru-RU")
				};

				options.DefaultRequestCulture = new RequestCulture("en-US", "en-US");
				options.SupportedCultures = supportedCultures;
				options.SupportedUICultures = supportedCultures;
			});

			services.AddDistributedMemoryCache();
			services.AddSession(options => { options.IdleTimeout = TimeSpan.FromDays(1); });
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ApplicationDbContext db)
		{
			db.Database.Migrate();

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

			app.UseStaticFiles();

#if HTTPS
			EnableHttps(app);
#endif

			app.UseAuthentication();

			app.UseSession();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					"default",
					"{controller=Home}/{action=Index}/{id?}");
			});
		}

		private static IConfigurationProvider CreateMapperConfiguration()
		{
			var mapperConfiguration = new MapperConfiguration(config =>
			{
				config.CreateMap<ApplicationUser, UserVM>()
					.ForMember(d => d.Name, opt => opt.MapFrom(s => s.UserName));

				config.CreateMap<Purchase, EditPurchaseVM>()
					.ForMember(d => d.Author, opt => opt.MapFrom(s => s.Author))
					.ForMember(d => d.LastEditor, opt => opt.MapFrom(s => s.LastEditor));

				config.CreateMap<Plan, PlanVM>()
					.ForMember(d => d.IsBought, opt => opt.MapFrom(s => s.BuyDate.HasValue));

				config.CreateMap<Category, CategoryVM>()
					.ForMember(d => d.DisplayName, opt => opt.MapFrom(s => s.Name))
					.ForMember(d => d.Color, opt => opt.MapFrom(s => s.Color.ToString("X6")));

				config.CreateMap<Purchase, PurchaseVM>()
					.ForMember(d => d.Category, opt => opt.MapFrom(s => s.Category));
			});

			mapperConfiguration.AssertConfigurationIsValid();
			return mapperConfiguration;
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