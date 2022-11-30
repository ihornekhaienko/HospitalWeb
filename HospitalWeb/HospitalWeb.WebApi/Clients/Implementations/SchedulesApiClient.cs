using HospitalWeb.DAL.Entities;
using HospitalWeb.WebApi.Models.ResourceModels;
using System.Net.Http.Headers;

namespace HospitalWeb.WebApi.Clients.Implementations
{
    public class SchedulesApiClient : GenericApiClient<Schedule, ScheduleResourceModel, int>
    {
        public SchedulesApiClient(IConfiguration config) : base(config, "Schedules")
        {
        }

        public HttpResponseMessage Get(string doctor, string day, string token = null, string provider = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_client.BaseAddress}{_addressSuffix}/details?doctor={doctor}&day={day}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("Provider", provider);

            return _client.SendAsync(request).Result;
        }
    }
}
