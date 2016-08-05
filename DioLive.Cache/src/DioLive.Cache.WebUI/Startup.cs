using System;
using System.Globalization;
using AutoMapper;
using DioLive.Cache.WebUI.Data;
using DioLive.Cache.WebUI.Models;
using DioLive.Cache.WebUI.Models.PurchaseViewModels;
using DioLive.Cache.WebUI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DioLive.Cache.WebUI
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

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

            services.AddMvc(options =>
            {
                options.ModelBinderProviders.Insert(0, new Binders.DateTimeModelBinderProvider());
            })
                .AddViewLocalization()
                .AddDataAnnotationsLocalization();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddSingleton(Localization.PurchasesPluralizer);
            services.AddSingleton(ApplicationOptions.Load());

            var mapperConfiguration = new MapperConfiguration(config =>
                {
                    config.CreateMap<ApplicationUser, UserVM>()
                        .ForMember(d => d.Name, opt => opt.ResolveUsing(s => s.UserName));

                    config.CreateMap<CreatePurchaseVM, Purchase>()
                        .ForMember(d => d.Id, opt => opt.ResolveUsing(_ => Guid.NewGuid()))
                        .ForMember(d => d.CreateDate, opt => opt.ResolveUsing(_ => DateTime.UtcNow))
                        .ForMember(d => d.AuthorId, opt => opt.Ignore())
                        .ForMember(d => d.Author, opt => opt.Ignore())
                        .ForMember(d => d.LastEditorId, opt => opt.Ignore())
                        .ForMember(d => d.LastEditor, opt => opt.Ignore())
                        .ForMember(d => d.BudgetId, opt => opt.Ignore())
                        .ForMember(d => d.Budget, opt => opt.Ignore())
                        .ForMember(d => d.Category, opt => opt.Ignore());

                    config.CreateMap<Purchase, EditPurchaseVM>()
                        .ForMember(d => d.Author, opt => opt.MapFrom(s => s.Author))
                        .ForMember(d => d.LastEditor, opt => opt.MapFrom(s => s.LastEditor));
                });
            mapperConfiguration.AssertConfigurationIsValid();

            services.AddSingleton<IMapper>(new Mapper(mapperConfiguration));

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("ru")
                };

                options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            services.AddDistributedMemoryCache();
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

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

            app.UseIdentity();

            app.UseSession();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}