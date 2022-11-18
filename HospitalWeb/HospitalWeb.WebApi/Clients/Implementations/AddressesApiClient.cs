using HospitalWeb.DAL.Entities;
using HospitalWeb.WebApi.Clients.Interfaces;

namespace HospitalWeb.WebApi.Clients.Implementations
{
    public class AddressesApiClient : ApiClient<Address, int>
    {
        public AddressesApiClient(IConfiguration config) : base(config)
        {
        }

        public override HttpResponseMessage Get()
        {
            return _client.GetAsync("Addresses").Result;
        }

        public override HttpResponseMessage Get(int identifier)
        {
            return _client.GetAsync($"Addresses/{identifier}").Result;
        }

        public override Address Read(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<Address>().Result;
        }

        public override HttpResponseMessage Post(Address obj)
        {
            return _client.PostAsJsonAsync("Addresses", obj).Result;
        }

        public override HttpResponseMessage Put(Address obj)
        {
            return _client.PutAsJsonAsync("Addresses", obj).Result;
        }

        public override HttpResponseMessage Delete(Address obj)
        {
            return _client.DeleteAsync($"Addresses/{obj}").Result;
        }

        public override HttpResponseMessage Delete(int identifier)
        {
            return _client.DeleteAsync($"Addresses/{identifier}").Result;
        }
    }
}
