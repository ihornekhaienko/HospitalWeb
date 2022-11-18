using HospitalWeb.DAL.Entities;
using HospitalWeb.WebApi.Clients.Interfaces;

namespace HospitalWeb.WebApi.Clients.Implementations
{
    public class SpecialtiesApiClient : ApiClient<Specialty, int>
    {
        public SpecialtiesApiClient(IConfiguration config) : base(config)
        {
        }

        public override HttpResponseMessage Get()
        {
            return _client.GetAsync("Specialties").Result;
        }

        public override HttpResponseMessage Get(int identifier)
        {
            return _client.GetAsync($"Specialties/{identifier}").Result;
        }

        public override Specialty Read(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<Specialty>().Result;
        }

        public override HttpResponseMessage Post(Specialty obj)
        {
            return _client.PostAsJsonAsync("Specialties", obj).Result;
        }

        public override HttpResponseMessage Put(Specialty obj)
        {
            return _client.PutAsJsonAsync("Specialties", obj).Result;
        }

        public override HttpResponseMessage Delete(Specialty obj)
        {
            return _client.DeleteAsync($"Specialties/{obj}").Result;
        }

        public override HttpResponseMessage Delete(int identifier)
        {
            return _client.DeleteAsync($"Specialties/{identifier}").Result;
        }
    }
}
