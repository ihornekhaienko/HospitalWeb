using HospitalWeb.Mvc.Attributes.Validation;
using HospitalWeb.Mvc.Services.Implementations;
using HospitalWeb.Mvc.Services.Interfaces;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MvcServiceCollectionExtensions
    {
        public static void AddScheduleGenerator(this IServiceCollection services)
        {
            services.AddScoped<IScheduleGenerator, ScheduleGenerator>();
        }

        public static void AddAttributesLocalization(this IServiceCollection services)
        {
            services.AddSingleton<ErrorMessageTranslationService>();
        }
    }
}
