using HospitalWeb.Domain.Data;
using HospitalWeb.Domain.Entities.Identity;
using HospitalWeb.Domain.Services.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HospitalWeb.Mvc;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;
using HospitalWeb.Mvc.Hubs;
using Google.Apis.Auth.AspNetCore3;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

#region LOCALIZATION
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews(o => o.Filters.Add(typeof(PollyExceptionFilter)))
    .AddDataAnnotationsLocalization(o =>
        o.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(SharedResource)))
    .AddViewLocalization();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en"),
        new CultureInfo("uk")
    };

    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

builder.Services.AddMultilangIdentityErrorDescriberFactory();
#endregion

#region DB
string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connection);
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders()
    .AddTokenProvider<DataProtectorTokenProvider<AppUser>>(TokenOptions.DefaultProvider)
    .AddMultilangIdentityErrorDescriber();
#endregion

#region SIGNALR
builder.Services.AddSignalR();
#endregion

#region AUTHORIZATION
builder.Services.AddAuthentication(o =>
{
    o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddGoogleOpenIdConnect("Google", options =>   
     {
         options.Authority = configuration["OAuth:Google:Authority"];
         options.CallbackPath = configuration["OAuth:Google:CallbackPath"];
         options.ClientId = configuration["OAuth:Google:ClientId"];
         options.ClientSecret = configuration["OAuth:Google:ClientSecret"];
         options.ResponseType = OpenIdConnectResponseType.Code;

         options.UsePkce = true;
         options.SaveTokens = true;

         options.Scope.Add(OpenIdConnectScope.OpenId);
         options.Scope.Add(OpenIdConnectScope.Email);

         options.Events.OnRedirectToIdentityProvider = context =>
         {
             context.ProtocolMessage.SetParameter("access_type", "offline");
             return Task.CompletedTask;
         };
     })
    .AddFacebook("Facebook", options =>
    {
        options.ClientId = configuration["OAuth:Facebook:ClientId"];
        options.ClientSecret = configuration["OAuth:Facebook:ClientSecret"];
        options.UsePkce = true;
        options.SaveTokens = true;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/Account/Login";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    });

builder.Services.AddAuthenticatorKeyService();
#endregion

builder.Services.AddApiClients();
builder.Services.AddApi();
builder.Services.AddEmailNotifier();
builder.Services.AddPasswordGenerator();
builder.Services.AddFileManager();
builder.Services.AddScheduleGenerator();
builder.Services.AddPdfPrinter();
builder.Services.AddZoom();
builder.Services.AddGoogleTokenProvider();
builder.Services.AddInternalGoogleProvider();
builder.Services.AddTokenManager();
builder.Services.AddGoogleCalendar();
builder.Services.AddLiqPayClient();

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRequestLocalization();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<NotificationHub>("/NotificationHub");
    endpoints.MapHub<RatingHub>("/RatingHub");
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
