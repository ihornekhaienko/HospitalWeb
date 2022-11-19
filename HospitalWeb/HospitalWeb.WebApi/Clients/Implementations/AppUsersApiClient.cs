using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.WebApi.Clients.Interfaces;
using HospitalWeb.WebApi.Models.ResourceModels;
using Microsoft.AspNetCore.Identity;

namespace HospitalWeb.WebApi.Clients.Implementations
{
    public class AppUsersApiClient : ApiClient<AppUser, string>
    {
        public AppUsersApiClient(IConfiguration config) : base(config)
        {
        }

        public override HttpResponseMessage Get()
        {
            return _client.GetAsync("AppUsers").Result;
        }

        public override HttpResponseMessage Get(string identifier)
        {
            return _client.GetAsync($"AppUsers/{identifier}").Result;
        }

        public override AppUser Read(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<AppUser>().Result;
        }

        public override AppUser Read(string identifier)
        {
            var response = Get(identifier);
            return Read(response);
        }

        public override IEnumerable<AppUser> ReadMany(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<IEnumerable<AppUser>>().Result;
        }

        public IEnumerable<IdentityError> ReadErrors(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<IEnumerable<IdentityError>>().Result;
        }

        public override HttpResponseMessage Post(AppUser obj)
        {
            var model = new AppUserResourceModel
            {
                Name = obj.Name,
                Surname = obj.Surname,
                Email = obj.Email,
                UserName = obj.Email,
                PhoneNumber = obj.PhoneNumber
            };

            return _client.PostAsJsonAsync("AppUsers", model).Result;
        }

        public HttpResponseMessage Post(AppUserResourceModel obj)
        {
            return _client.PostAsJsonAsync("AppUsers", obj).Result;
        }

        public override HttpResponseMessage Put(AppUser obj)
        {
            var model = new AppUserResourceModel
            {
                Name = obj.Name,
                Surname = obj.Surname,
                Email = obj.Email,
                UserName = obj.Email,
                PhoneNumber = obj.PhoneNumber,
                Image = obj.Image
            };
            return _client.PutAsJsonAsync("AppUsers", model).Result;
        }

        public HttpResponseMessage Put(AppUserResourceModel obj)
        {
            return _client.PutAsJsonAsync("AppUsers", obj).Result;
        }

        public override HttpResponseMessage Delete(AppUser obj)
        {
            return _client.DeleteAsync($"AppUsers/{obj}").Result;
        }

        public override HttpResponseMessage Delete(string identifier)
        {
            return _client.DeleteAsync($"AppUsers/{identifier}").Result;
        }
    }
}
