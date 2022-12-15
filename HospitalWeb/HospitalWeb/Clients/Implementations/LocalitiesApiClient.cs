using HospitalWeb.DAL.Entities;
using HospitalWeb.WebApi.Models.ResourceModels;
using System.Net.Http.Headers;

namespace HospitalWeb.Clients.Implementations
{
    public class LocalitiesApiClient : GenericApiClient<Locality, LocalityResourceModel, int>
    {
        public LocalitiesApiClient(IConfiguration config) : base(config, "Localities")
        {
        }

        public HttpResponseMessage Get(string name, string token = null, string provider = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_client.BaseAddress}{_addressSuffix}/details?name={name}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("Provider", provider);

            return _client.SendAsync(request).Result;
        }

        public Locality GetOrCreate(string name, string token = null, string provider = null)
        {
            var response = Get(name, token, provider);

            if (response.IsSuccessStatusCode)
            {
                return Read(response);
            }
            else
            {
                var locality = new LocalityResourceModel
                {
                    LocalityName = name
                };

                return Read(Post(locality, token, provider));
            }
        }
    }
}
