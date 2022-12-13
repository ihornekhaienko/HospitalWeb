using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.Services.Interfaces;
using HospitalWeb.Services.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HospitalWeb.Services.Implementations
{
    public class InternalTokenProvider : ITokenProvider
    {
        private readonly IConfiguration _config;
        private readonly UserManager<AppUser> _userManager;
        private ITokenProvider _successor;

        public ITokenProvider Successor { get => _successor; set => _successor = value; }

        public InternalTokenProvider(
            IConfiguration config, 
            UserManager<AppUser> userManager)
        {
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

                if (user is Admin admin)
                {
                    if (admin.IsSuperAdmin)
                    {
                        claims.Add(new Claim("AccessLevel", "Super"));
                    }
                    else
                    {
                        claims.Add(new Claim("AccessLevel", "Basic"));
                    }
                }

                ClaimsIdentity claimsIdentity =
                    new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);

                return claimsIdentity;
            }

            return null;
        }

        public async Task<string> RefreshToken(AppUser user)
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

        public async Task<TokenResult> GetToken(AppUser user)
        {
            var token = await RefreshToken(user);

            return new TokenResult { Token = token, Provider = "Internal" };
        }
    }
}
