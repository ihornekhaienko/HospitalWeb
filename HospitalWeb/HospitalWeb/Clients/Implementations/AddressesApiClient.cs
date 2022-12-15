using HospitalWeb.DAL.Entities;
using HospitalWeb.WebApi.Models.ResourceModels;
using System.Net.Http.Headers;

namespace HospitalWeb.Clients.Implementations
{
    public class AddressesApiClient : GenericApiClient<Address, AddressResourceModel, int>
    {
        public AddressesApiClient(IConfiguration config, IHttpClientFactory clientFactory) 
            : base(config, clientFactory, "Addresses")
        {
        }

        public HttpResponseMessage Get(string address, string locality, string token = null, string provider = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_client.BaseAddress}{_addressSuffix}/details?address={address}&locality={locality}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("Provider", provider);

            return _client.SendAsync(request).Result;
        }

        public Address GetOrCreate(string address, Locality locality, string token = null, string provider = null)
        {
            var response = Get(address, locality.LocalityName, token, provider);

            if (response.IsSuccessStatusCode)
            {
                return Read(response);
            }
            else
            {
                var obj = new AddressResourceModel
                {
                    FullAddress = address,
                    LocalityId = locality.LocalityId
                };

                return Read(Post(obj, token, provider));
            }
        }
    }
}
