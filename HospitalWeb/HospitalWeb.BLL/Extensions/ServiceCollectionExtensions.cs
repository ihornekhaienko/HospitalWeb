using HospitalWeb.Domain.Entities.Identity;
using HospitalWeb.Domain.Services.Interfaces;
using HospitalWeb.Services.Implementations;
using HospitalWeb.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
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

        public static void AddAuthenticatorKeyService(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticatorKeyService, AuthenticatorKeyService>();
        }

        public static void AddLiqPayClient(this IServiceCollection services)
        {
            services.AddScoped<ILiqPayClient, LiqPayClient>();
        }
    }
}
