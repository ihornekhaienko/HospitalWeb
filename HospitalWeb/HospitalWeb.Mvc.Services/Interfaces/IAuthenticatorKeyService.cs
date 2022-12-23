using HospitalWeb.Domain.Entities.Identity;

namespace HospitalWeb.Mvc.Services.Interfaces
{
    public interface IAuthenticatorKeyService
    {
        public Task<string> LoadSharedKey(AppUser user);

        public string LoadQrCodeUri(AppUser user, string sharedKey);
    }
}
