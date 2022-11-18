using HospitalWeb.DAL.Entities;
using HospitalWeb.WebApi.Clients.Interfaces;

namespace HospitalWeb.WebApi.Clients.Implementations
{
    public class LocalitiesApiClient : ApiClient<Locality, int>
    {
        public LocalitiesApiClient(IConfiguration config) : base(config)
        {
        }

        public override HttpResponseMessage Get()
        {
            return _client.GetAsync("Localities").Result;
        }

        public override HttpResponseMessage Get(int identifier)
        {
            return _client.GetAsync($"Localities/{identifier}").Result;
        }

        public override Locality Read(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<Locality>().Result;
        }

        public override HttpResponseMessage Post(Locality obj)
        {
            return _client.PostAsJsonAsync("Localities", obj).Result;
        }

        public override HttpResponseMessage Put(Locality obj)
        {
            return _client.PutAsJsonAsync("Localities", obj).Result;
        }

        public override HttpResponseMessage Delete(Locality obj)
        {
            return _client.DeleteAsync($"Localities/{obj}").Result;
        }

        public override HttpResponseMessage Delete(int identifier)
        {
            return _client.DeleteAsync($"Localities/{identifier}").Result;
        }
    }
}
