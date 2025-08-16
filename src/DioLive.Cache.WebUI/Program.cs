using System.Globalization;

using DioLive.Cache.Binder;
using DioLive.Cache.Common;
using DioLive.Cache.Common.Localization;
using DioLive.Cache.WebUI.Binders;
using DioLive.Cache.WebUI.Models;

using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews(options =>
{
    options.ModelBinderProviders.Insert(0, new DateTimeModelBinderProvider());
    options.ModelBinderProviders.Insert(1, new DecimalModelBinderProvider());
})
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

builder.Services.AddSingleton<WordLocalizer>();
builder.Services.AddSingleton(new ApplicationOptions
{
    Version = builder.Configuration["Version"] ?? "0.1.0"
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddScoped<ICurrentContext, HttpCurrentContext>();

builder.Services.BindCacheDependencies(builder.Configuration);

builder.Services.Configure<RequestLocalizationOptions>(options => ConfigureLocalization(
    builder.Configuration,
    options
));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => { options.IdleTimeout = TimeSpan.FromDays(1); });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.UseSession();
app.UseRequestLocalization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
return;

static void ConfigureLocalization(
    IConfiguration configuration,
    RequestLocalizationOptions options
)
{
    var supportedCultures = configuration.GetRequiredSection("SupportedCultures").Get<SupportedCulture[]>() ?? [];

    CultureInfo[] cultureInfos =
    [
        .. supportedCultures.Select(culture => new CultureInfo(culture.Code))
    ];

    CultureInfo defaultCulture = cultureInfos[0];

    options.DefaultRequestCulture = new RequestCulture(defaultCulture, defaultCulture);
    options.SupportedCultures = cultureInfos;
    options.SupportedUICultures = cultureInfos;

    Cultures.Default = defaultCulture.Name;
    Cultures.Supported = supportedCultures;
}