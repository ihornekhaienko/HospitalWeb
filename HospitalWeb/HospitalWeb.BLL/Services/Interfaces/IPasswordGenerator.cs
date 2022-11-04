using Microsoft.AspNetCore.Identity;

namespace HospitalWeb.BLL.Services.Interfaces
{
    public interface IPasswordGenerator
    {
        string? GeneratePassword(PasswordOptions? opts);
    }
}
