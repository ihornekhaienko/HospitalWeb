using Azure.Security.KeyVault.Secrets;
using Azure.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Google.Apis.Auth.AspNetCore3;
using HospitalWeb.Domain.Data;
using HospitalWeb.Domain.Entities.Identity;
using HospitalWeb.Domain.Services.Extensions;
using HospitalWeb.Mvc;
using HospitalWeb.Mvc.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Globalization;


var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var secretClient = new SecretClient(new Uri(config["AZURE_KEY_VAULT"]), new DefaultAzureCredential());
config.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());


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
string connection = config.GetConnectionString("Default");
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
         options.Authority = config["OAuth:Google:Authority"];
         options.CallbackPath = config["OAuth:Google:CallbackPath"];
         options.ClientId = config["OAuth:Google:ClientId"];
         options.ClientSecret = config["OAuth:Google:ClientSecret"];
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
        options.ClientId = config["OAuth:Facebook:ClientId"];
        options.ClientSecret = config["OAuth:Facebook:ClientSecret"];
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
