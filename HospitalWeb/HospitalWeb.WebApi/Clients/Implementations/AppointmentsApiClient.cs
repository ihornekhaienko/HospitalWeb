using HospitalWeb.DAL.Entities;
using HospitalWeb.WebApi.Clients.Interfaces;
using HospitalWeb.WebApi.Models.SortStates;
using System.Text;

namespace HospitalWeb.WebApi.Clients.Implementations
{
    public class AppointmentsApiClient : ApiClient<Appointment, int>
    {
        public AppointmentsApiClient(IConfiguration config) : base(config)
        {
        }

        public override HttpResponseMessage Get()
        {
            return _client.GetAsync("Appointments").Result;
        }

        public override HttpResponseMessage Get(int identifier)
        {
            return _client.GetAsync($"Appointments/{identifier}").Result;
        }

        public HttpResponseMessage Get(string doctor, DateTime date)
        {
            return _client.GetAsync($"Appointments/details?doctor={doctor}&date={date}").Result;
        }

        public HttpResponseMessage Filter(
            string searchString = null,
            string userId = null,
            int? state = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            AppointmentSortState sortOrder = AppointmentSortState.DateAsc,
            int pageSize = 10,
            int pageNumber = 1)
        {
            return _client.GetAsync($"Appointments?searchString={searchString}&userId={userId}&state={state}&fromDate={fromDate}&toDate={toDate}" +
                $"&sortOrder={sortOrder}&pageSize={pageSize}&pageNumber={pageNumber}").Result;
        }

        public override Appointment Read(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<Appointment>().Result;
        }

        public override Appointment Read(int identifier)
        {
            var response = Get(identifier);
            return Read(response);
        }

        public override IEnumerable<Appointment> ReadMany(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<IEnumerable<Appointment>>().Result;
        }

        public override HttpResponseMessage Post(Appointment obj)
        {
            return _client.PostAsJsonAsync("Appointments", obj).Result;
        }

        public override HttpResponseMessage Put(Appointment obj)
        {
            return _client.PutAsJsonAsync("Appointments", obj).Result;
        }

        public override HttpResponseMessage Delete(Appointment obj)
        {
            return _client.DeleteAsync($"Appointments/{obj}").Result;
        }

        public override HttpResponseMessage Delete(int identifier)
        {
            return _client.DeleteAsync($"Appointments/{identifier}").Result;
        }

        public bool IsDateFree(string doctor, DateTime date)
        {
            var response = Get(doctor, date);

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

        public void UpdateStates()
        {
            var date = DateTime.Today.AddDays(-1);

            var response = Filter(toDate: date);

            if (response.IsSuccessStatusCode)
            {
                var appointments = ReadMany(response);

                foreach (var appointment in appointments)
                {
                    if (appointment.State == State.Planned)
                    {
                        appointment.State = State.Missed;
                        Put(appointment);
                    }    
                }
            }
        }
    }
}
