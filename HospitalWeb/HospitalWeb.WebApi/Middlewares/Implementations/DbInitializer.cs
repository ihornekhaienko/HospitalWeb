using HospitalWeb.Domain.Data;
using HospitalWeb.WebApi.Services.Interfaces;

namespace HospitalWeb.WebApi.Middlewares.Implementations
{
    public class DbInitializer
    {
        private readonly RequestDelegate _next;

        public DbInitializer(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context, 
            AppDbContext db, 
            IDbInitializer dbInitializer)
        {
            ArgumentNullException.ThrowIfNull(db, nameof(db));

            await dbInitializer.Init();

            await _next.Invoke(context);
        }
    }
}
