using HospitalWeb.Domain.Entities;
using HospitalWeb.Mvc.Models.ResourceModels;
using System.Globalization;
using System.Net.Http.Headers;

namespace HospitalWeb.Mvc.Clients.Implementations
{
    public class MessagesApiClient : GenericApiClient<Message, MessageResourceModel, int>
    {
        public MessagesApiClient(IConfiguration config, IHttpClientFactory clientFactory)
            : base(config, clientFactory, "Messages")
        {
        }

        public HttpResponseMessage Filter(
            string user = null, 
            int pageSize = 10,
            int pageNumber = 1,
            string token = null, 
            string provider = null)
        {
            string query = $"details?user={user}&pageSize={pageSize}&pageNumber={pageNumber}";
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_client.BaseAddress}{_addressSuffix}/{query}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("Provider", provider);
            request.Headers.Add("Accept-Language", CultureInfo.CurrentCulture.Name);

            return _client.SendAsync(request).Result;
        }
    }
}
