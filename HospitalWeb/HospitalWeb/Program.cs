using HospitalWeb.Services.Extensions;
using HospitalWeb.DAL.Data;
using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Extensions;
using HospitalWeb.Middlewares.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HospitalWeb;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using HospitalWeb.WebApi.Clients.Extensions;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;
using HospitalWeb.Hubs;
using Google.Apis.Auth.AspNetCore3;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

#region DB
string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connection);
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddTokenProvider<DataProtectorTokenProvider<AppUser>>(TokenOptions.DefaultProvider);
#endregion

#region SIGNALR
builder.Services.AddSignalR();
#endregion

#region LOCALIZATION
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews()
    .AddDataAnnotationsLocalization(options =>
        options.DataAnnotationLocalizerProvider = (type, factory) =>
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
#endregion

#region AUTHORIZATION
builder.Services.AddAuthentication(o =>
{
    o.DefaultChallengeScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
    o.DefaultForbidScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
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

#endregion

builder.Services.AddUnitOfWork();
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
builder.Services.AddDbInitializer();

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseDbInitializer();
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
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
