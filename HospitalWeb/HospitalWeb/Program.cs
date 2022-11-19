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

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connection));

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddTokenProvider<DataProtectorTokenProvider<AppUser>>(TokenOptions.DefaultProvider);

builder.Services.AddApi();
builder.Services.AddEmailNotifier();
builder.Services.AddPasswordGenerator();
builder.Services.AddFileManager();
builder.Services.AddScheduleGenerator();

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

builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = configuration["Authentication:Google:ClientId"];
        options.ClientSecret = configuration["Authentication:Google:ClientSecret"];
    })
    .AddFacebook(options =>
    {
        options.ClientId = configuration["Authentication:Facebook:ClientId"];
        options.ClientSecret = configuration["Authentication:Facebook:ClientSecret"];
    });

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

app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
