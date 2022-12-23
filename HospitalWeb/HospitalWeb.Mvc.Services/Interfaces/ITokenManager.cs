using HospitalWeb.Domain.Entities.Identity;
using HospitalWeb.Mvc.Services.Utility;

namespace HospitalWeb.Mvc.Services.Interfaces
{
    public interface ITokenManager
    {
        public Task<TokenResult> GetToken(AppUser user);
    }
}
