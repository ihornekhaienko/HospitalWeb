using HospitalWeb.Domain.Entities.Identity;
using HospitalWeb.Mvc.Services.Utility;

namespace HospitalWeb.Mvc.Services.Interfaces
{
    public interface ITokenProvider
    {
        public ITokenProvider Successor { get; set; }

        public Task<TokenResult> GetToken(AppUser user);

        public Task<string> RefreshToken(AppUser user);
    }
}
