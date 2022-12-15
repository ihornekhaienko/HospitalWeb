using HospitalWeb.DAL.Entities;
using HospitalWeb.WebApi.Models.ResourceModels;
using System.Net.Http.Headers;

namespace HospitalWeb.Clients.Implementations
{
    public class SpecialtiesApiClient : GenericApiClient<Specialty, SpecialtyResourceModel, int>
    {
        public SpecialtiesApiClient(IConfiguration config, IHttpClientFactory clientFactory) 
            : base(config, clientFactory, "Specialties")
        {
        }

        public HttpResponseMessage Get(string name, string token = null, string provider = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_client.BaseAddress}{_addressSuffix}/details?name={name}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("Provider", provider);

            return _client.SendAsync(request).Result;
        }

        public Specialty GetOrCreate(string name, string token = null, string provider = null)
        {
            var response = Get(name, token, provider);

            if (response.IsSuccessStatusCode)
            {
                return Read(response);
            }
            else
            {
                var specialty = new SpecialtyResourceModel
                {
                    SpecialtyName = name
                };

                return Read(Post(specialty, token, provider));
            }
        }
    }
}
