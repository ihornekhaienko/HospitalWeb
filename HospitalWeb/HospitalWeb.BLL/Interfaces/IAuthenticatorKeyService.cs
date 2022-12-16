using Microsoft.AspNetCore.Identity;

namespace HospitalWeb.Services.Interfaces
{
    public interface IAuthenticatorKeyService
    {
        public Task<string> LoadSharedKey(IdentityUser user);

        public string LoadQrCodeUri(IdentityUser user, string sharedKey);
    }
}
