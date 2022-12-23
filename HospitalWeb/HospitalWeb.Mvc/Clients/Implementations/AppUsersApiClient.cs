using HospitalWeb.Domain.Entities.Identity;
using HospitalWeb.Mvc.Models.ResourceModels;

namespace HospitalWeb.Mvc.Clients.Implementations
{
    public class AppUsersApiClient : GenericApiClient<AppUser, AppUserResourceModel, string>
    {
        public AppUsersApiClient(IConfiguration config, IHttpClientFactory clientFactory) 
            : base(config, clientFactory, "AppUsers")
        {
        }
    }
}
