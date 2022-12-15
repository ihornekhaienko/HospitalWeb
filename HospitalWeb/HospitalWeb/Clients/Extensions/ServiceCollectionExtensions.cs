using HospitalWeb.Clients.Implementations;

namespace HospitalWeb.Clients.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApi(this IServiceCollection services)
        {
            services.AddScoped<ApiUnitOfWork>();
        }
    }
}
