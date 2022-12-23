using HospitalWeb.Domain.Entities.Identity;
using HospitalWeb.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Text;
using System.Text.Encodings.Web;

namespace HospitalWeb.Services.Implementations
{
    public class AuthenticatorKeyService : IAuthenticatorKeyService
    {
        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        private readonly UserManager<AppUser> _userManager;
        private readonly UrlEncoder _urlEncoder; 

        public AuthenticatorKeyService(UserManager<AppUser> userManager, UrlEncoder urlEncoder)
        {
            _userManager = userManager;
            _urlEncoder = urlEncoder;
        }

        public string LoadQrCodeUri(AppUser user, string sharedKey)
        {
            return GenerateQrCodeUri(user.Email, sharedKey);
        }

        public async Task<string> LoadSharedKey(AppUser user)
        {
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            return FormatKey(unformattedKey);
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                AuthenticatorUriFormat,
                _urlEncoder.Encode("HospitalWeb"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }
    }
}
