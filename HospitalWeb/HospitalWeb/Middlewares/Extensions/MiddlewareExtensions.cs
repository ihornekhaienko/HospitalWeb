using HospitalWeb.Middlewares.Implementations;

namespace HospitalWeb.Middlewares.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseDbInitializer(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DbInitializer>();
        }
    }
}
