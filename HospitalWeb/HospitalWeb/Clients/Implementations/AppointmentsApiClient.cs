using HospitalWeb.DAL.Entities;
using HospitalWeb.WebApi.Models.ResourceModels;
using HospitalWeb.WebApi.Models.SortStates;
using System.Net.Http.Headers;

namespace HospitalWeb.Clients.Implementations
{
    public class AppointmentsApiClient : GenericApiClient<Appointment, AppointmentResourceModel, int>
    {
        public AppointmentsApiClient(IConfiguration config, IHttpClientFactory clientFactory) 
            : base(config, clientFactory, "Appointments")
        {
        }

        public HttpResponseMessage Get(string doctor, DateTime date, string token = null, string provider = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_client.BaseAddress}{_addressSuffix}/details?doctor={doctor}&date={date}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("Provider", provider);

            return _client.SendAsync(request).Result;
        }

        public HttpResponseMessage Filter(
            string searchString = null,
            string userId = null,
            int? state = null,
            int? locality = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            AppointmentSortState sortOrder = AppointmentSortState.DateAsc,
            int pageSize = 10,
            int pageNumber = 1,
            string token = null,
            string provider = null)
        {
            string query = $"?searchString={searchString}&userId={userId}&state={state}&locality={locality}&fromDate={fromDate}&toDate={toDate}" +
                $"&sortOrder={sortOrder}&pageSize={pageSize}&pageNumber={pageNumber}";

            var request = new HttpRequestMessage(HttpMethod.Get, $"{_client.BaseAddress}{_addressSuffix}{query}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("Provider", provider);

            return _client.SendAsync(request).Result;
        }

        public bool IsDateFree(string doctor, DateTime date, string token = null, string provider = null)
        {
            var response = Get(doctor, date, token, provider);

            if (response.IsSuccessStatusCode)
            {
                var appointment = Read(response);

                if (appointment.State == State.Planned || appointment.State == State.Active || appointment.State == State.Completed)
                {
                    return false;
                }
            }

            return true;
        }

        public HttpResponseMessage PutMany(IEnumerable<Appointment> items, string token = null, string provider = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"{_client.BaseAddress}{_addressSuffix}");
            HttpContent httpContent = JsonContent.Create(items);

            request.Content = httpContent;
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("Provider", provider);

            return _client.SendAsync(request).Result;
        }
    }
}
