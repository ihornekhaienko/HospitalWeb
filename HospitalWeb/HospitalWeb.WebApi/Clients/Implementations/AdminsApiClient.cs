using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.Filters.Models.SortStates;
using HospitalWeb.WebApi.Clients.Interfaces;

namespace HospitalWeb.WebApi.Clients.Implementations
{
    public class AdminsApiClient : ApiClient<Admin, string>
    {
        public AdminsApiClient(IConfiguration config) : base(config)
        {
        }

        public override HttpResponseMessage Get()
        {
            return _client.GetAsync("Admins").Result;
        }

        public HttpResponseMessage Get(
            string searchString,
            AdminSortState sortOrder = AdminSortState.Id,
            int pageSize = 10,
            int pageNumber = 1)
        {
            return _client.GetAsync($"Admins?searchString={searchString}&sortOrder={sortOrder}&pageSize={pageSize}&pageNumber={pageNumber}").Result;
        }

        public override HttpResponseMessage Get(string identifier)
        {
            return _client.GetAsync($"Admins/{identifier}").Result;
        }

        public override Admin Read(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<Admin>().Result;
        }

        public override HttpResponseMessage Post(Admin obj)
        {
            return _client.PostAsJsonAsync("Admins", obj).Result;
        }

        public override HttpResponseMessage Put(Admin obj)
        {
            return _client.PutAsJsonAsync("Admins", obj).Result;
        }

        public override HttpResponseMessage Delete(Admin obj)
        {
            return _client.DeleteAsync($"Admins/{obj}").Result;
        }

        public override HttpResponseMessage Delete(string identifier)
        {
            return _client.DeleteAsync($"Admins/{identifier}").Result;
        }
    }
}
