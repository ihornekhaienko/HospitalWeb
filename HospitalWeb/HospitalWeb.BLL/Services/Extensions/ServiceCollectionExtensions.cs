using HospitalWeb.BLL.Services.Implementations;
using HospitalWeb.BLL.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HospitalWeb.BLL.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddEmailService(this IServiceCollection services)
        {
            services.AddScoped<IEmailService, EmailService>();
        }

        public static void AddPasswordGenerator(this IServiceCollection services)
        {
            services.AddScoped<IPasswordGenerator, PasswordGenerator>();
        }
    }
}
