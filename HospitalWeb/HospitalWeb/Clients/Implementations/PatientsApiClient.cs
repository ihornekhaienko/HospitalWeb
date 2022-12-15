using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.WebApi.Models.ResourceModels;
using HospitalWeb.WebApi.Models.SortStates;
using Microsoft.AspNetCore.Identity;
using System.Net.Http.Headers;

namespace HospitalWeb.Clients.Implementations
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

            return _client.SendAsync(request).Result;
        }
    }
}
