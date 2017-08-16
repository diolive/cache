using System;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

using DioLive.BlackMint.Logic;
using DioLive.BlackMint.Logic.Implementation;
using DioLive.BlackMint.Persistence;
using DioLive.BlackMint.Persistence.SQLite;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

using ILogger = DioLive.BlackMint.Persistence.ILogger;

namespace DioLive.BlackMint.WebApp
{
    public class Startup
    {
        private readonly IConfigurationRoot _configuration;

        public Startup(IHostingEnvironment env)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            _configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            IConfigurationSection auth0Config = _configuration.GetSection("Auth0");
            IConfigurationSection dataConfig = _configuration.GetSection("Data");

            services.Configure<Auth0Settings>(auth0Config);
            services.Configure<DataSettings>(dataConfig);

            var auth0Settings = auth0Config.Get<Auth0Settings>();
            var dataSettings = dataConfig.Get<DataSettings>();

            ConfigureDependencyInjection(services, dataSettings);

            services.AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultChallengeScheme = "Auth0";
                })
                .AddCookie()
                .AddOpenIdConnect("Auth0", options =>
                {
                    options.Authority = $"https://{auth0Settings.Domain}";
                    options.ClientId = auth0Settings.ClientId;
                    options.ClientSecret = auth0Settings.ClientSecret;
                    options.ResponseType = OpenIdConnectResponseType.IdToken;
                    options.CallbackPath = new PathString("/signin-auth0");
                    options.ClaimsIssuer = "Auth0";

                    options.Events.OnRedirectToIdentityProviderForSignOut = context =>
                    {
                        string logoutUri =
                            $"https://{auth0Settings.Domain}/v2/logout?client_id={auth0Settings.ClientId}";

                        string postLogoutUri = context.Properties.RedirectUri;
                        if (!string.IsNullOrEmpty(postLogoutUri))
                        {
                            if (postLogoutUri.StartsWith("/"))
                            {
                                HttpRequest request = context.Request;
                                postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase +
                                                postLogoutUri;
                            }
                            logoutUri += $"&returnTo={Uri.EscapeDataString(postLogoutUri)}";
                        }

                        context.Response.Redirect(logoutUri);
                        context.HandleResponse();

                        return Task.CompletedTask;
                    };

                    options.Events.OnRedirectToIdentityProvider = context =>
                    {
                        context.ProtocolMessage.SetParameter("audience", "https://dio.auth0.com/api/v2/");

                        return Task.CompletedTask;
                    };

                    options.Events.OnTokenValidated = async context =>
                    {
                        string nameIdentity = context.SecurityToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

                        HttpContext httpContext = context.Request.HttpContext;
                        var identityLogic = httpContext.RequestServices.GetService<IIdentityLogic>();

                        await identityLogic.GetOrCreateUser(nameIdentity, () => GetDisplayName(context.SecurityToken));
                    };

                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("email");
                });

            services.AddMvc();

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(1);
                options.Cookie.HttpOnly = true;
            });
        }

        private void ConfigureDependencyInjection(IServiceCollection services, DataSettings dataSettings)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IIdentityStorage, IdentityStorage>();
            services.AddSingleton<IDomainStorage, DomainStorage>();
            services.AddSingleton<ILogger, Logger>();

            services.AddSingleton<IIdentityLogic, IdentityLogic>();
            services.AddSingleton<IDomainLogic, DomainLogic>();

            services.AddSingleton(new SqliteConnection(dataSettings.ConnectionString));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
                              IOptions<Auth0Settings> auth0Options, IOptions<DataSettings> dataOptions)
        {
            loggerFactory.AddConsole(_configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private static string GetDisplayName(JwtSecurityToken securityToken)
        {
            string[] subClaim = securityToken.Claims.First(c => c.Type == "sub").Value.Split(new[] { '|' }, 2);
            switch (subClaim[0])
            {
                case "auth0": //email
                    return securityToken.Claims.First(c => c.Type == "email").Value;

                case "vkontakte": //vk
                    string givenName = securityToken.Claims.First(c => c.Type == "given_name").Value;
                    string familyName = securityToken.Claims.First(c => c.Type == "family_name").Value;
                    return $"{givenName} {familyName}";

                case "google-oauth2": //google
                    return securityToken.Claims.First(c => c.Type == "name").Value;

                default:
                    return subClaim[1];
            }
        }
    }
}