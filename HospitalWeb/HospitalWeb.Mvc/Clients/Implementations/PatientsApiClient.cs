using HospitalWeb.Domain.Entities.Identity;
using HospitalWeb.Mvc.Models.ResourceModels;
using HospitalWeb.Mvc.Models.SortStates;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using System.Net.Http.Headers;

namespace HospitalWeb.Mvc.Clients.Implementations
{
    public class PatientsApiClient : GenericApiClient<Patient, PatientResourceModel, string>
    {
        public PatientsApiClient(IConfiguration config, IHttpClientFactory clientFactory) 
            : base(config, clientFactory, "Patients")
        {
        }

        public HttpResponseMessage Filter(
            string searchString,
            int? locality,
            PatientSortState sortOrder = PatientSortState.Id,
            int pageSize = 10,
            int pageNumber = 1,
            string token = null,
            string provider = null)
        {
            string query = $"?searchString={searchString}&locality={locality}&sortOrder={sortOrder}" +
                $"&pageSize={pageSize}&pageNumber={pageNumber}";
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_client.BaseAddress}{_addressSuffix}{query}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("Provider", provider);
            request.Headers.Add("Accept-Language", CultureInfo.CurrentCulture.Name);

            return _client.SendAsync(request).Result;
        }
    }
}
