using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace HospitalWeb.WebApi.Utility.TokenValidators
{
    public class FacebookTokenValidator : ISecurityTokenValidator
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _apiEndpoint;

        private readonly JwtSecurityTokenHandler _tokenHandler;

        public FacebookTokenValidator(string clientId, string clientSecret, string apiEndpoint)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _apiEndpoint = apiEndpoint;

            _tokenHandler = new JwtSecurityTokenHandler();
        }

        public bool CanValidateToken => true;

        public int MaximumTokenSizeInBytes { get; set; } = TokenValidationParameters.DefaultMaximumTokenSizeInBytes;

        public bool CanReadToken(string securityToken)
        {
            return true;
        }

        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            validatedToken = null;
            try
            {
                var validationParams = new Dictionary<string, string>
                {
                    { "input_token", securityToken },
                    { "access_token", $"{_clientId}|{_clientSecret}" }
                };

                string query = $"{_apiEndpoint}?input_token={securityToken}&access_token={_clientId}|{_clientSecret}";
                var httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, query);

                var response = httpClient.Send(request);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException();
                }

                var resultJson = response.Content.ReadAsStringAsync().Result;
                dynamic data = JsonConvert.DeserializeObject<dynamic>(resultJson).data;
                var email = data.scopes[0].Value;

                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, "Patient"),
                };

                var principle = new ClaimsPrincipal();
                principle.AddIdentity(new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme));

                validatedToken = new JwtSecurityToken(claims: principle.Claims);

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
