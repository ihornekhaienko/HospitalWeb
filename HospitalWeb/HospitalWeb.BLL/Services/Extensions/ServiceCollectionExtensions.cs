using HospitalWeb.BLL.Services.Implementations;
using HospitalWeb.BLL.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HospitalWeb.BLL.Services.Extensions
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
    }
}
