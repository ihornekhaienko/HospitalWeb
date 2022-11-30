using HospitalWeb.DAL.Entities;
using HospitalWeb.WebApi.Models.ResourceModels;
using System.Net.Http.Headers;

namespace HospitalWeb.WebApi.Clients.Implementations
{
    public class MeetingsApiClient : GenericApiClient<Meeting, MeetingResourceModel, int>
    {
        public MeetingsApiClient(IConfiguration config) : base(config, "Meetings")
        {
        }

        public HttpResponseMessage GetByAppointment(int appointmentId, string token = null, string provider = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_client.BaseAddress}{_addressSuffix}/details?{appointmentId}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("Provider", provider);

            return _client.SendAsync(request).Result;
        }
    }
}
