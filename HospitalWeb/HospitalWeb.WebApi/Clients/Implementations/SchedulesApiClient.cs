using HospitalWeb.DAL.Entities;
using HospitalWeb.WebApi.Clients.Interfaces;

namespace HospitalWeb.WebApi.Clients.Implementations
{
    public class SchedulesApiClient : ApiClient<Schedule, int>
    {
        public SchedulesApiClient(IConfiguration config) : base(config)
        {
        }

        public override HttpResponseMessage Get()
        {
            return _client.GetAsync("Schedules").Result;
        }

        public override HttpResponseMessage Get(int identifier)
        {
            return _client.GetAsync($"Schedules/{identifier}").Result;
        }

        public HttpResponseMessage Get(string doctor, string day)
        {
            return _client.GetAsync($"Schedules/details?doctor={doctor}&day={day}").Result;
        }

        public override Schedule Read(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<Schedule>().Result;
        }

        public override Schedule Read(int identifier)
        {
            var response = Get(identifier);
            return Read(response);
        }

        public override IEnumerable<Schedule> ReadMany(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<IEnumerable<Schedule>>().Result;
        }

        public override HttpResponseMessage Post(Schedule obj)
        {
            return _client.PostAsJsonAsync("Schedules", obj).Result;
        }

        public override HttpResponseMessage Put(Schedule obj)
        {
            return _client.PutAsJsonAsync("Schedules", obj).Result;
        }

        public override HttpResponseMessage Delete(Schedule obj)
        {
            return _client.DeleteAsync($"Schedules/{obj}").Result;
        }

        public override HttpResponseMessage Delete(int identifier)
        {
            return _client.DeleteAsync($"Schedules/{identifier}").Result;
        }
    }
}
