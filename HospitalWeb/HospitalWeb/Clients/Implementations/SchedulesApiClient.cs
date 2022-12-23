using HospitalWeb.DAL.Entities;
using HospitalWeb.Models.ResourceModels;
using System.Globalization;
using System.Net.Http.Headers;

namespace HospitalWeb.Clients.Implementations
{
    public class SchedulesApiClient : GenericApiClient<Schedule, ScheduleResourceModel, int>
    {
        public SchedulesApiClient(IConfiguration config, IHttpClientFactory clientFactory) 
            : base(config, clientFactory, "Schedules")
        {
        }

        public HttpResponseMessage Get(string doctor, string day, string token = null, string provider = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_client.BaseAddress}{_addressSuffix}/details?doctor={doctor}&day={day}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("Provider", provider);
            request.Headers.Add("Accept-Language", CultureInfo.CurrentCulture.Name);

            return _client.SendAsync(request).Result;
        }
    }
}
