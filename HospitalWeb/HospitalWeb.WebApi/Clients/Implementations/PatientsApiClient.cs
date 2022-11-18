using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.Filters.Models.SortStates;
using HospitalWeb.WebApi.Clients.Interfaces;

namespace HospitalWeb.WebApi.Clients.Implementations
{
    public class PatientsApiClient : ApiClient<Patient, string>
    {
        public PatientsApiClient(IConfiguration config) : base(config)
        {
        }

        public override HttpResponseMessage Get()
        {
            return _client.GetAsync("Patients").Result;
        }

        public HttpResponseMessage Get(
            string searchString,
            int? locality,
            PatientSortState sortOrder = PatientSortState.Id,
            int pageSize = 10,
            int pageNumber = 1)
        {
            return _client.GetAsync($"Patients?searchString={searchString}&locality={locality}&sortOrder={sortOrder}" +
                $"&pageSize={pageSize}&pageNumber={pageNumber}").Result;
        }

        public override HttpResponseMessage Get(string identifier)
        {
            return _client.GetAsync($"Patients/{identifier}").Result;
        }

        public override Patient Read(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<Patient>().Result;
        }

        public override HttpResponseMessage Post(Patient obj)
        {
            return _client.PostAsJsonAsync("Patients", obj).Result;
        }

        public override HttpResponseMessage Put(Patient obj)
        {
            return _client.PutAsJsonAsync("Patients", obj).Result;
        }

        public override HttpResponseMessage Delete(Patient obj)
        {
            return _client.DeleteAsync($"Patients/{obj}").Result;
        }

        public override HttpResponseMessage Delete(string identifier)
        {
            return _client.DeleteAsync($"Patients/{identifier}").Result;
        }
    }
}
