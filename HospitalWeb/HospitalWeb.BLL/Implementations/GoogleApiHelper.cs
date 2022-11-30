using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.Services.Interfaces;
using HospitalWeb.Services.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HospitalWeb.Services.Implementations
{
    public class GoogleApiHelper : IGoogleApiHelper
    {
        private readonly ILogger<GoogleApiHelper> _logger;
        private readonly IConfiguration _config;
        private readonly UserManager<AppUser> _userManager;

        public GoogleApiHelper(ILogger<GoogleApiHelper> logger, IConfiguration config, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _config = config;
            _userManager = userManager;
        }

        public async Task<string> RefreshToken(AppUser user)
        {
            var refreshToken = await _userManager.GetAuthenticationTokenAsync(user, "Google", "refresh_token");

            if (refreshToken == null)
            {
                return null;
            }

            var refreshParams = new Dictionary<string, string>
            {
                { "client_id", _config["OAuth:Google:ClientId"] },
                { "client_secret", _config["OAuth:Google:ClientSecret"] },
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken }
            };

            var httpContent = new FormUrlEncodedContent(refreshParams);
            var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, _config["OAuth:Google:TokenEndpoint"]);
            request.Content = httpContent;
            var response = await httpClient.SendAsync(request);

            var resultJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(resultJson);
            }

            var token = JsonConvert.DeserializeObject<GoogleToken>(resultJson);

            if (token == null)
            {
                return null;
            }

            await _userManager.RemoveAuthenticationTokenAsync(user, "Google", "access_token");
            await _userManager.SetAuthenticationTokenAsync(user, "Google", "access_token", token.AccessToken);
            await _userManager.RemoveAuthenticationTokenAsync(user, "Google", "id_token");
            await _userManager.SetAuthenticationTokenAsync(user, "Google", "id_token", token.IdToken);
            await _userManager.RemoveAuthenticationTokenAsync(user, "Google", "expires_at");
            await _userManager.SetAuthenticationTokenAsync(user, "Google", "expires_at", token.ExpiresIn);

            return token.IdToken;
        }
    }
}
