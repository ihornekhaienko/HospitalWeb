using HospitalWeb.Domain.Entities.Identity;
using HospitalWeb.Services.Interfaces;
using HospitalWeb.Services.Utility;

namespace HospitalWeb.Services.Implementations
{
    public class TokenManager : ITokenManager
    {
        private readonly IList<ITokenProvider> _tokenProviders;

        public TokenManager(IEnumerable<ITokenProvider> tokenProviders)
        {
            _tokenProviders = new List<ITokenProvider>(tokenProviders);

            for (int i = 0; i < _tokenProviders.Count; i++)
            {
                if (i != _tokenProviders.Count - 1)
                {
                    _tokenProviders[i].Successor = _tokenProviders[i + 1];
                }
            }
        }

        public async Task<TokenResult> GetToken(AppUser user)
        {
            return await _tokenProviders.FirstOrDefault()?.GetToken(user);
        }
    }
}
