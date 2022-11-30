using HospitalWeb.DAL.Entities.Identity;

namespace HospitalWeb.Services.Interfaces
{
    public interface ITokenManager
    {
        public Task<string> GenerateToken(AppUser user);
    }
}
