using HospitalWeb.WebApi.Clients.Implementations;

namespace HospitalWeb.WebApi.Clients.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApi(this IServiceCollection services)
        {
            services.AddScoped<ApiUnitOfWork>();
        }
    }
}
