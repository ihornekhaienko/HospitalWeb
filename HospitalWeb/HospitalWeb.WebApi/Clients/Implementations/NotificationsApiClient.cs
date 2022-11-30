using HospitalWeb.DAL.Entities;
using HospitalWeb.WebApi.Models.ResourceModels;
using System.Net.Http.Headers;

namespace HospitalWeb.WebApi.Clients.Implementations
{
    public class NotificationsApiClient : GenericApiClient<Notification, NotificationResourceModel, int>
    {
        public NotificationsApiClient(IConfiguration config) : base(config, "Notifications")
        {
        }

        public HttpResponseMessage Filter(
            string owner, 
            int pageSize = 10, 
            int pageNumber = 1,
            string token = null,
            string provider = null)
        {
            string query = $"/details?owner={owner}&pageSize={pageSize}&pageNumber={pageNumber}";
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_client.BaseAddress}{_addressSuffix}{query}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("Provider", provider);

            return _client.SendAsync(request).Result;
        }
    }
}
