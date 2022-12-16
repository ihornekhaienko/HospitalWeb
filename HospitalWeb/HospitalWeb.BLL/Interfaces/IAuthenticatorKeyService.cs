using HospitalWeb.DAL.Entities.Identity;

namespace HospitalWeb.Services.Interfaces
{
    public interface IAuthenticatorKeyService
    {
        public Task<string> LoadSharedKey(AppUser user);

        public string LoadQrCodeUri(AppUser user, string sharedKey);
    }
}
