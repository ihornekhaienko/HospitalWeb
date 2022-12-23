using HospitalWeb.Domain.Entities.Identity;
using HospitalWeb.Services.Utility;

namespace HospitalWeb.Services.Interfaces
{
    public interface ITokenManager
    {
        public Task<TokenResult> GetToken(AppUser user);
    }
}
