using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Google.Apis.Auth;

namespace HospitalWeb.WebApi.Authentication.Google
{
    public class GoogleTokenValidator : ISecurityTokenValidator
    {
        private readonly string _clientId;
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public GoogleTokenValidator(string clientId)
        {
            _clientId = clientId;
            _tokenHandler = new JwtSecurityTokenHandler();
        }

        public bool CanValidateToken => true;

        public int MaximumTokenSizeInBytes { get; set; } = TokenValidationParameters.DefaultMaximumTokenSizeInBytes;

        public bool CanReadToken(string securityToken)
        {
            return _tokenHandler.CanReadToken(securityToken);
        }

        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            validatedToken = null;

            try
            {
                var payload = GoogleJsonWebSignature.ValidateAsync(securityToken, new GoogleJsonWebSignature.ValidationSettings() { Audience = new[] { _clientId } }).Result; // here is where I delegate to Google to validate
                validatedToken = _tokenHandler.ReadJwtToken(securityToken);

                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, payload.Name),
                        new Claim(ClaimTypes.Name, payload.Name),
                        new Claim(JwtRegisteredClaimNames.FamilyName, payload.FamilyName),
                        new Claim(JwtRegisteredClaimNames.GivenName, payload.GivenName),
                        new Claim(JwtRegisteredClaimNames.Email, payload.Email),
                        new Claim(JwtRegisteredClaimNames.Sub, payload.Subject),
                        new Claim(JwtRegisteredClaimNames.Iss, payload.Issuer),
                    };

                var principle = new ClaimsPrincipal();
                principle.AddIdentity(new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme));

                return principle;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
        }
    }
}