using Microsoft.AspNetCore.Identity;

namespace HospitalWeb.Services.Interfaces
{
    public interface IPasswordGenerator
    {
        string? GeneratePassword(PasswordOptions? opts);
    }
}
