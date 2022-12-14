using HospitalWeb.Domain.Entities;
using HospitalWeb.Mvc.Models.ResourceModels;
using HospitalWeb.Mvc.Models.SortStates;
using System.Globalization;
using System.Net.Http.Headers;

namespace HospitalWeb.Mvc.Clients.Implementations
{
    public class HospitalsApiClient : GenericApiClient<Hospital, HospitalResourceModel, int>
    {
        public HospitalsApiClient(IConfiguration config, IHttpClientFactory clientFactory) 
            : base(config, clientFactory, "Hospitals")
        {
        }

        public HttpResponseMessage Get(string name, string token = null, string provider = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_client.BaseAddress}{_addressSuffix}/details?name={name}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("Provider", provider);
            request.Headers.Add("Accept-Language", CultureInfo.CurrentCulture.Name);

            return _client.SendAsync(request).Result;
        }

        public Hospital GetOrCreate(string name, string token = null, string provider = null)
        {
            var response = Get(name, token, provider);

            if (response.IsSuccessStatusCode)
            {
                return Read(response);
            }
            else
            {
                var hospital = new HospitalResourceModel
                {
                    HospitalName = name
                };

                return Read(Post(hospital, token, provider));
            }
        }

        public HttpResponseMessage Filter(
            string searchString,
            int? locality,
            int? type,
            HospitalSortState sortOrder = HospitalSortState.Id,
            int pageSize = 10,
            int pageNumber = 1,
            string token = null,
            string provider = null)
        {
            string query = $"?searchString={searchString}&locality={locality}&type={type}&sortOrder={sortOrder}" +
                $"&pageSize={pageSize}&pageNumber={pageNumber}";
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_client.BaseAddress}{_addressSuffix}{query}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("Provider", provider);
            request.Headers.Add("Accept-Language", CultureInfo.CurrentCulture.Name);

            return _client.SendAsync(request).Result;
        }
    }
}
