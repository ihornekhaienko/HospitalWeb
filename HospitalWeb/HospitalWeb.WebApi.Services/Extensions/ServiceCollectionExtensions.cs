using HospitalWeb.Domain.Entities.Identity;
using HospitalWeb.Domain.Services.Interfaces;
using HospitalWeb.WebApi.Services.Implementations;
using HospitalWeb.WebApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDbInitializer(this IServiceCollection services, bool fullGenerate = false)
        {
            services.AddScoped<IDbInitializer, HospitalDbInitializer>(x =>
                new HospitalDbInitializer(x.GetRequiredService<IConfiguration>(),
                                          x.GetRequiredService<UserManager<AppUser>>(),
                                          x.GetRequiredService<RoleManager<IdentityRole>>(),
                                          x.GetRequiredService<IUnitOfWork>(),
                                          fullGenerate));
        }
    }
}
