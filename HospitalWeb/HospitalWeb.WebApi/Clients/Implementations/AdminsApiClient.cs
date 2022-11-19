using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.WebApi.Clients.Interfaces;
using HospitalWeb.WebApi.Models.ResourceModels;
using HospitalWeb.WebApi.Models.SortStates;
using Microsoft.AspNetCore.Identity;

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

        public HttpResponseMessage Filter(
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

        public override Admin Read(string identifier)
        {
            var response = Get(identifier);
            return Read(response);
        }

        public override IEnumerable<Admin> ReadMany(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<IEnumerable<Admin>>().Result;
        }

        public IEnumerable<IdentityError> ReadErrors(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<IEnumerable<IdentityError>>().Result;
        }

        public override HttpResponseMessage Post(Admin obj)
        {
            var model = new AdminResourceModel
            {
                Name = obj.Name,
                Surname = obj.Surname,
                Email = obj.Email,
                UserName = obj.Email,
                PhoneNumber = obj.PhoneNumber,
                IsSuperAdmin = obj.IsSuperAdmin,
                EmailConfirmed = obj.EmailConfirmed
            };

            return _client.PostAsJsonAsync("Admins", model).Result;
        }

        public HttpResponseMessage Post(AdminResourceModel obj)
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
