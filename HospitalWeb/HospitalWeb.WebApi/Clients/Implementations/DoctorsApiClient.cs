using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.WebApi.Clients.Interfaces;
using HospitalWeb.WebApi.Models.SortStates;

namespace HospitalWeb.WebApi.Clients.Implementations
{
    public class DoctorsApiClient : ApiClient<Doctor, string>
    {
        public DoctorsApiClient(IConfiguration config) : base(config)
        {
        }

        public override HttpResponseMessage Get()
        {
            return _client.GetAsync("Doctors").Result;
        }

        public HttpResponseMessage Get(
            string searchString,
            int? specialty,
            DoctorSortState sortOrder = DoctorSortState.Id,
            int pageSize = 10,
            int pageNumber = 1)
        {
            return _client.GetAsync($"Doctors?searchString={searchString}&specialty={specialty}&sortOrder={sortOrder}" +
                $"&pageSize={pageSize}&pageNumber={pageNumber}").Result;
        }

        public override HttpResponseMessage Get(string identifier)
        {
            return _client.GetAsync($"Doctors/{identifier}").Result;
        }

        public override Doctor Read(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<Doctor>().Result;
        }

        public override HttpResponseMessage Post(Doctor obj)
        {
            return _client.PostAsJsonAsync("Doctors", obj).Result;
        }

        public override HttpResponseMessage Put(Doctor obj)
        {
            return _client.PutAsJsonAsync("Doctors", obj).Result;
        }

        public override HttpResponseMessage Delete(Doctor obj)
        {
            return _client.DeleteAsync($"Doctors/{obj}").Result;
        }

        public override HttpResponseMessage Delete(string identifier)
        {
            return _client.DeleteAsync($"Doctors/{identifier}").Result;
        }
    }
}
