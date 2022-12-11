using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Interfaces;
using HospitalWeb.Services.Implementations;
using HospitalWeb.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HospitalWeb.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddEmailNotifier(this IServiceCollection services)
        {
            services.AddScoped<INotifier, EmailNotifier>();
        }

        public static void AddPasswordGenerator(this IServiceCollection services)
        {
            services.AddScoped<IPasswordGenerator, PasswordGenerator>();
        }

        public static void AddFileManager(this IServiceCollection services)
        {
            services.AddScoped<IFileManager, FileManager>();
        }

        public static void AddPdfPrinter(this IServiceCollection services)
        {
            services.AddScoped<IPdfPrinter, PdfPrinter>();
        }

        public static void AddZoom(this IServiceCollection services)
        {
            services.AddScoped<IMeetingService, ZoomMeetingService>();
        }

        public static void AddGoogleTokenProvider(this IServiceCollection services)
        {
            services.AddScoped<ITokenProvider, GoogleTokenProvider>();
        }

        public static void AddInternalGoogleProvider(this IServiceCollection services)
        {
            services.AddScoped<ITokenProvider, InternalTokenProvider>();
        }

        public static void AddTokenManager(this IServiceCollection services)
        {
            services.AddScoped<ITokenManager, TokenManager>();
        }

        public static void AddGoogleCalendar(this IServiceCollection services)
        {
            services.AddScoped<ICalendarService, GoogleCalendarService>();
        }

        public static void AddDbInitializer(this IServiceCollection services, bool fullGenerate = false)
        {
            services.AddScoped<IDbInitializer, HospitalDbInitializer>(x =>
                new HospitalDbInitializer(x.GetRequiredService<IConfiguration>(),
                                          x.GetRequiredService<UserManager<AppUser>>(),
                                          x.GetRequiredService<RoleManager<IdentityRole>>(),
                                          x.GetRequiredService<IUnitOfWork>(),
                                          fullGenerate));
        }
    }
}
