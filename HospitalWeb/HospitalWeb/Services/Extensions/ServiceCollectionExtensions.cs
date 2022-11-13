using HospitalWeb.Services.Implementations;
using HospitalWeb.Services.Interfaces;

namespace HospitalWeb.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddScheduleGenerator(this IServiceCollection services)
        {
            services.AddScoped<IScheduleGenerator, ScheduleGenerator>();
        }
    }
}
