using HospitalWeb.DAL.Entities.Identity;

namespace HospitalWeb.Services.Interfaces
{
    public interface IGoogleApiHelper
    {
        public Task<string> RefreshToken(AppUser user);
    }
}
