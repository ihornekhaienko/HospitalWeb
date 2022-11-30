using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HospitalWeb.Services.Implementations
{
    public class TokenManager : ITokenManager
    {
        private readonly ILogger<TokenManager> _logger;
        private readonly IConfiguration _config;
        private readonly UserManager<AppUser> _userManager;

        public TokenManager(
            ILogger<TokenManager> logger, 
            IConfiguration config, 
            UserManager<AppUser> userManager)
        {
            _logger = logger;
            _config = config;
            _userManager = userManager;
        }

        private async Task<ClaimsIdentity> GetIdentity(AppUser user)
        {
            if (user != null)
            {
                var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, role)
                };

                ClaimsIdentity claimsIdentity =
                    new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);

                return claimsIdentity;
            }

            return null;
        }

        public async Task<string> GenerateToken(AppUser user)
        {
            var identity = await GetIdentity(user);

            if (identity == null)
            {
                throw new Exception("User not found");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["JWT:Key"]));
            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                claims: identity.Claims,
                notBefore: now,
                expires: now.AddMinutes(Convert.ToDouble(_config["JWT:Lifetime"])),
                signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
                );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            await _userManager.RemoveAuthenticationTokenAsync(user, "Custom", "access_token");
            await _userManager.SetAuthenticationTokenAsync(user, "Custom", "access_token", encodedJwt);

            return encodedJwt;
        }
    }
}
