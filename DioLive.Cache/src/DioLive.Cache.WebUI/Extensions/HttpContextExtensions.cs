using Microsoft.AspNetCore.Localization;

namespace Microsoft.AspNetCore.Http
{
    public static class HttpContextExtensions
    {
        public static string GetCurrentCulture(this HttpContext httpContext)
        {
            return httpContext.Features.Get<IRequestCultureFeature>().RequestCulture.UICulture.Name;
        }
    }
}