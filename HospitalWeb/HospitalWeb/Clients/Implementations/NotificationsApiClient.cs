using HospitalWeb.Domain.Entities;
using HospitalWeb.Models.ResourceModels;
using System.Globalization;
using System.Net.Http.Headers;

namespace HospitalWeb.Clients.Implementations
{
    public class NotificationsApiClient : GenericApiClient<Notification, NotificationResourceModel, int>
    {
        public NotificationsApiClient(IConfiguration config, IHttpClientFactory clientFactory) 
            : base(config, clientFactory, "Notifications")
        {
        }

        public HttpResponseMessage Filter(
            string owner, 
            bool? isRead = null,
            int pageSize = 10, 
            int pageNumber = 1,
            string token = null,
            string provider = null)
        {
            string query = $"/details?owner={owner}&isRead={isRead}&pageSize={pageSize}&pageNumber={pageNumber}";
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_client.BaseAddress}{_addressSuffix}{query}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("Provider", provider);
            request.Headers.Add("Accept-Language", CultureInfo.CurrentCulture.Name);

            return _client.SendAsync(request).Result;
        }
    }
}
