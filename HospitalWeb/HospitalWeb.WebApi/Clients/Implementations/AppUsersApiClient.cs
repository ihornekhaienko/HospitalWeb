using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.WebApi.Clients.Interfaces;
using HospitalWeb.WebApi.Models.ResourceModels;
using Microsoft.AspNetCore.Identity;

namespace HospitalWeb.WebApi.Clients.Implementations
{
    public class AppUsersApiClient : ApiClient<AppUser, AppUserResourceModel, string>
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

        public override HttpResponseMessage Post(AppUserResourceModel obj)
        {
            return _client.PostAsJsonAsync("AppUsers", obj).Result;
        }

        public override HttpResponseMessage Put(AppUserResourceModel obj)
        {
            return _client.PutAsJsonAsync("AppUsers", obj).Result;
        }

        public override HttpResponseMessage Delete(string identifier)
        {
            return _client.DeleteAsync($"AppUsers/{identifier}").Result;
        }
    }
}
