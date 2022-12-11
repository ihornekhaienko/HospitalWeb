using HospitalWeb.DAL.Data;
using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace HospitalWeb.Middlewares.Implementations
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
