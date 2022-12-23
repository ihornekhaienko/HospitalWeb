using Microsoft.AspNetCore.Identity;

namespace HospitalWeb.Mvc.Services.Interfaces
{
    public interface IPasswordGenerator
    {
        string GeneratePassword(PasswordOptions opts);
    }
}
