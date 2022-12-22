using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.WebApi.Models.ResourceModels;
using HospitalWeb.WebApi.Models.SortStates;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using System.Net.Http.Headers;

namespace HospitalWeb.Clients.Implementations
{
    public class AdminsApiClient : GenericApiClient<Admin, AdminResourceModel, string>
    {
        public AdminsApiClient(IConfiguration config, IHttpClientFactory clientFactory) 
            : base(config, clientFactory, "Admins")
        {
        }

        public HttpResponseMessage Filter(
            string searchString,
            AdminSortState sortOrder = AdminSortState.Id,
            int pageSize = 10,
            int pageNumber = 1,
            string token = null,
            string provider = null)
        {
            string query = $"?searchString={searchString}&sortOrder={sortOrder}&pageSize={pageSize}&pageNumber={pageNumber}";
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_client.BaseAddress}{_addressSuffix}{query}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("Provider", provider);
            request.Headers.Add("Accept-Language", CultureInfo.CurrentCulture.Name);

            return _client.SendAsync(request).Result;
        }
    }
}
