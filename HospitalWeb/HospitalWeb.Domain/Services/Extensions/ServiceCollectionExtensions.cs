using HospitalWeb.Domain.Services.Implementations;
using HospitalWeb.Domain.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HospitalWeb.Domain.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
