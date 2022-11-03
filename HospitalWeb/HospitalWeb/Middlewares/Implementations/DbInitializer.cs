using HospitalWeb.DAL.Data;
using HospitalWeb.DAL.Entities.Identity;
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

        public async Task InvokeAsync(HttpContext context, AppDbContext db, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            ArgumentNullException.ThrowIfNull(db, nameof(db));

            if (await roleManager.FindByNameAsync("Admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (await roleManager.FindByNameAsync("Patient") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Patient"));
            }
            if (await roleManager.FindByNameAsync("Doctor") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Doctor"));
            }

            if (await userManager.FindByEmailAsync("ihornekhaienko@gmail.com") == null)
            {
                string email = "ihornekhaienko@gmail.com";
                string password = "Pass_1111";

                var admin = new Admin
                {
                    Name = "Ihor",
                    Surname = "Nekhaienko",
                    UserName = email,
                    Email = email,
                    PhoneNumber = "+380500558375",
                    EmailConfirmed = true,
                    IsSuperAdmin = true
                };

                var result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

            await _next.Invoke(context);
        }
    }
}
