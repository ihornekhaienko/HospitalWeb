using HospitalWeb.Domain.Entities.Identity;
using HospitalWeb.Mvc.Models.ResourceModels;
using HospitalWeb.Mvc.Models.SortStates;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using System.Net.Http.Headers;

namespace HospitalWeb.Mvc.Clients.Implementations
{
    public class DoctorsApiClient : GenericApiClient<Doctor, DoctorResourceModel, string>
    {
        public DoctorsApiClient(IConfiguration config, IHttpClientFactory clientFactory) 
            : base(config, clientFactory, "Doctors")
        {
        }

        public HttpResponseMessage Filter(
            string searchString,
            int? specialty,
            int? hospital,
            int? locality,
            DoctorSortState sortOrder = DoctorSortState.Id,
            int pageSize = 10,
            int pageNumber = 1,
            string token = null,
            string provider = null)
        {
            string query = $"?searchString={searchString}&specialty={specialty}&hospital={hospital}&locality={locality}" +
                $"&sortOrder={sortOrder}&pageSize={pageSize}&pageNumber={pageNumber}";

            var request = new HttpRequestMessage(HttpMethod.Get, $"{_client.BaseAddress}{_addressSuffix}{query}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("Provider", provider);
            request.Headers.Add("Accept-Language", CultureInfo.CurrentCulture.Name);

            return _client.SendAsync(request).Result;
        }
    }
}
