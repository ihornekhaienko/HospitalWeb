using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.Services.Utility;

namespace HospitalWeb.Services.Interfaces
{
    public interface ITokenProvider
    {
        public ITokenProvider Successor { get; set; }

        public Task<TokenResult> GetToken(AppUser user);

        public Task<string> RefreshToken(AppUser user);
    }
}
