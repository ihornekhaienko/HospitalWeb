using HospitalWeb.DAL.Services.Implementations;
using HospitalWeb.DAL.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HospitalWeb.DAL.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAddressService(this IServiceCollection services)
        {
            services.AddScoped<IAddressService, AddressService>();
        }

        public static void AddLocalityService(this IServiceCollection services)
        {
            services.AddScoped<ILocalityService, LocalityService>();
        }
    }
}
