using HospitalWeb.Services.Implementations;
using HospitalWeb.Services.Interfaces;
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
    }
}
