using System;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

using Dapper.Contrib.Extensions;

using DioLive.BlackMint.WebApp.Data;
using DioLive.BlackMint.WebApp.Models;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

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
            services.AddAuthentication(options => options.SignInScheme =
                CookieAuthenticationDefaults.AuthenticationScheme);

            services.AddMvc();

            services.AddOptions();

            services.Configure<Auth0Settings>(_configuration.GetSection("Auth0"));
            services.Configure<DataSettings>(_configuration.GetSection("Data"));

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(1);
                options.CookieHttpOnly = true;
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
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

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true
            });

            app.UseSession();

            var options = new OpenIdConnectOptions("Auth0")
            {
                Authority = $"https://{auth0Options.Value.Domain}",

                ClientId = auth0Options.Value.ClientId,
                ClientSecret = auth0Options.Value.ClientSecret,

                AutomaticAuthenticate = false,
                AutomaticChallenge = false,

                ResponseType = OpenIdConnectResponseType.IdToken, // "code",

                CallbackPath = new PathString("/signin-auth0"),

                ClaimsIssuer = "Auth0",

                Events = new OpenIdConnectEvents
                {
                    OnRedirectToIdentityProviderForSignOut = context =>
                    {
                        string logoutUri =
                            $"https://{auth0Options.Value.Domain}/v2/logout?client_id={auth0Options.Value.ClientId}";

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
                    },
                    OnRedirectToIdentityProvider = context =>
                    {
                        context.ProtocolMessage.SetParameter("audience", "https://dio.auth0.com/api/v2/");

                        return Task.FromResult(0);
                    },
                    OnTokenValidated = context =>
                    {
                        string nameIdentity = context.SecurityToken.Claims.FirstOrDefault(c => c.Type == "sub").Value;
                        using (var db = new SqlConnection(dataOptions.Value.ConnectionString))
                        {
                            string displayName;
                            int? userId = Database.GetUserIdByNameIdentity(db, nameIdentity).GetAwaiter().GetResult();

                            if (userId.HasValue)
                            {
                                displayName = Database.GetUserDisplayNameById(db, userId.Value).GetAwaiter()
                                    .GetResult();
                            }
                            else
                            {
                                displayName = GetDisplayName(context.SecurityToken);

                                var user = new User
                                {
                                    DisplayName = displayName
                                };
                                userId = (int)db.Insert(user);

                                var userIdentity = new UserIdentity
                                {
                                    UserId = user.Id,
                                    NameIdentity = nameIdentity
                                };
                                db.Insert(userIdentity);
                            }

                            ISession session = app.ApplicationServices.GetService<IHttpContextAccessor>()
                                .HttpContext.Session;
                            session.SetInt32("userId", userId.Value);
                            session.SetString("currentUser", displayName);
                        }

                        return Task.CompletedTask;
                    }
                }
            };

            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("email");
            app.UseOpenIdConnectAuthentication(options);

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