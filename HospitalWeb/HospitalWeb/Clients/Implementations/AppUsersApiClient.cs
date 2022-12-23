using HospitalWeb.Domain.Entities.Identity;
using HospitalWeb.Models.ResourceModels;

namespace HospitalWeb.Clients.Implementations
{
    public class AppUsersApiClient : GenericApiClient<AppUser, AppUserResourceModel, string>
    {
        public AppUsersApiClient(IConfiguration config, IHttpClientFactory clientFactory) 
            : base(config, clientFactory, "AppUsers")
        {
        }
    }
}
