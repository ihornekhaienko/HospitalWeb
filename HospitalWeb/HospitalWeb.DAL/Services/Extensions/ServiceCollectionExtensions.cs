using HospitalWeb.DAL.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace HospitalWeb.DAL.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<UnitOfWork>();
        }
    }
}
