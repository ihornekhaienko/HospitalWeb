using HospitalWeb.WebApi.Middlewares.Implementations;

namespace HospitalWeb.WebApi.Middlewares
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseDbInitializer(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DbInitializer>();
        }
    }
}
