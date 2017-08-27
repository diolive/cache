using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

using DioLive.BlackMint.Logic;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DioLive.BlackMint.WebApp.Extensions
{
    public static class Auth0Extensions
    {
        public static AuthenticationBuilder AddAuth0(this AuthenticationBuilder builder)
        {
            return builder.AddAuth0(_ => { });
        }

        public static AuthenticationBuilder AddAuth0(this AuthenticationBuilder builder,
                                                     Action<OpenIdConnectOptions> configureOptions)
        {
            builder.Services.Configure(configureOptions);
            builder.Services.AddSingleton<IConfigureOptions<OpenIdConnectOptions>, ConfigureAuth0Options>();
            builder.AddOpenIdConnect();
            return builder;
        }

        private static string GetDisplayName(JwtSecurityToken securityToken)
        {
            string[] subClaim = securityToken.Claims.First(c => c.Type == "sub").Value.Split(new[] { '|' }, 2);
            switch (subClaim[0])
            {
                case "auth0": //email
                    return securityToken.Claims.First(c => c.Type == "email").Value;

                case "vkontakte": //vk
                    string givenName = securityToken.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value;
                    string familyName = securityToken.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value;
                    return $"{givenName} {familyName}".Trim();

                case "google-oauth2": //google
                    return securityToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value;

                default:
                    return subClaim[1];
            }
        }

        private class ConfigureAuth0Options : IConfigureNamedOptions<OpenIdConnectOptions>
        {
            private readonly Auth0Settings _settings;

            public ConfigureAuth0Options(IOptions<Auth0Settings> auth0Options)
            {
                _settings = auth0Options.Value;
            }

            public void Configure(string name, OpenIdConnectOptions options)
            {
                Configure(options);
            }

            public void Configure(OpenIdConnectOptions options)
            {
                options.Authority = _settings.Domain;
                options.CallbackPath = _settings.CallbackUrl;
                options.ClaimsIssuer = "Auth0";
                options.ClientId = _settings.ClientId;
                options.ClientSecret = _settings.ClientSecret;
                options.ResponseType = "id_token";
                options.UseTokenLifetime = true;

                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");

                options.Events = new OpenIdConnectEvents
                {
                    OnRedirectToIdentityProvider = OnRedirectToIdentityProvider,
                    OnRedirectToIdentityProviderForSignOut = OnRedirectToIdentityProviderForSignOut,
                    OnTokenValidated = OnTokenValidated
                };
            }

            private Task OnRedirectToIdentityProvider(RedirectContext context)
            {
                context.ProtocolMessage.SetParameter("audience", $"{_settings.Domain}/api/v2/");

                return Task.CompletedTask;
            }

            private Task OnRedirectToIdentityProviderForSignOut(RedirectContext context)
            {
                string logoutUri = $"{_settings.Domain}/v2/logout?client_id={_settings.ClientId}";

                string postLogoutUri = context.Properties.RedirectUri;
                if (!string.IsNullOrEmpty(postLogoutUri))
                {
                    if (postLogoutUri.StartsWith("/"))
                    {
                        HttpRequest request = context.Request;
                        postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
                    }
                    logoutUri += $"&returnTo={Uri.EscapeDataString(postLogoutUri)}";
                }

                context.Response.Redirect(logoutUri);
                context.HandleResponse();

                return Task.CompletedTask;
            }

            private async Task OnTokenValidated(TokenValidatedContext context)
            {
                string nameIdentity = context.SecurityToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

                HttpContext httpContext = context.Request.HttpContext;
                var identityLogic = httpContext.RequestServices.GetService<IIdentityLogic>();

                await identityLogic.GetOrCreateUser(nameIdentity, () => GetDisplayName(context.SecurityToken));
            }
        }
    }
}