using HospitalWeb.Domain.Entities.Identity;
using HospitalWeb.Mvc.Services.Interfaces;
using HospitalWeb.Mvc.Services.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace HospitalWeb.Mvc.Services.Implementations
{
    public class FacebookTokenProvider : ITokenProvider
    {
        private readonly IConfiguration _config;
        private readonly UserManager<AppUser> _userManager;

        private ITokenProvider _successor;

        public ITokenProvider Successor { get => _successor; set => _successor = value; }

        public FacebookTokenProvider(
            IConfiguration config,
            UserManager<AppUser> userManager)
        {
            _config = config;
            _userManager = userManager;
        }

        public async Task<TokenResult> GetToken(AppUser user)
        {
            var token = await RefreshToken(user);

            if (token != null)
            {
                return new TokenResult { Token = token, Provider = "Facebook" };
            }

            if (Successor != null)
            {
                return await Successor.GetToken(user);
            }

            return null;
        }

        public async Task<string> RefreshToken(AppUser user)
        {
            var exchangeToken = await _userManager.GetAuthenticationTokenAsync(user, "Facebook", "access_token");

            if (exchangeToken == null)
            {
                return null;
            }

            var exchangeParams = new Dictionary<string, string>
            {
                { "client_id", _config["OAuth:Facebook:ClientId"] },
                { "client_secret", _config["OAuth:Facebook:ClientSecret"] },
                { "grant_type", "fb_exchange_token" },
                { "fb_exchange_token", exchangeToken }
            };

            var httpContent = new FormUrlEncodedContent(exchangeParams);
            var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, _config["OAuth:Facebook:TokenEndpoint"]);
            request.Content = httpContent;
            var response = await httpClient.SendAsync(request);

            var resultJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(resultJson);
            }

            var token = JsonConvert.DeserializeObject<FacebookToken>(resultJson);

            if (token == null)
            {
                return null;
            }

            await _userManager.RemoveAuthenticationTokenAsync(user, "Facebook", "access_token");
            await _userManager.SetAuthenticationTokenAsync(user, "Facebook", "access_token", token.AccessToken);
            await _userManager.RemoveAuthenticationTokenAsync(user, "Facebook", "expires_at");
            await _userManager.SetAuthenticationTokenAsync(user, "Facebook", "expires_at", token.ExpiresIn);
            await _userManager.RemoveAuthenticationTokenAsync(user, "Facebook", "token_type");
            await _userManager.SetAuthenticationTokenAsync(user, "Facebook", "token_type", token.TokenType);

            return token.AccessToken;
        }
    }
}
