using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace HospitalWeb.Services.Implementations
{
    public class GoogleCalendarService : ICalendarService
    {
        private readonly IConfiguration _config;
        private readonly CalendarService _service;

        public GoogleCalendarService(IConfiguration config)
        {
            _config = config;

            string[] scopes = { CalendarService.Scope.Calendar };

            var credential = new ServiceAccountCredential(
                new ServiceAccountCredential.Initializer(_config["Calendar:ClientEmail"])
                {
                    Scopes = scopes
                }.FromPrivateKey(_config["Calendar:PrivateKey"]));

            _service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "HospitalCalendar"
            });
        }

        public async Task<string> CreateCalendar()
        {
            var calendar = new Calendar()
            {
                Summary = "Appointments",
                Description = "Your appointments in HospitalDb"
            };

            return (await _service.Calendars.Insert(calendar).ExecuteAsync()).Id;
        }

        public string GetCalendar(AppUser user)
        {
            return _config["Calendar:CalendarUrl"] + user.CalendarId + _config["Calendar:TimeZone"];
        }

        public async Task CreateEvent(AppUser user, Appointment appointment)
        {
            var calendarId = user.CalendarId;

            var calendarEvent = new Event()
            {
                Id = "event" + appointment.AppointmentId.ToString(),
                Summary = $"Appointment with {appointment.Doctor.Specialty.SpecialtyName}",
                Description = $"Appointment with Dr. {appointment.Doctor} ({appointment.Doctor.Specialty.SpecialtyName}) " +
                    $"on {appointment.AppointmentDate.ToString("MM/dd/yyyy HH:mm")}",
                Start = new EventDateTime
                {
                    DateTime = appointment.AppointmentDate,
                    TimeZone = "Europe/Kyiv"
                },
                End = new EventDateTime
                {
                    DateTime = appointment.AppointmentDate.AddHours(1),
                    TimeZone = "Europe/Kyiv"
                },
                Attendees = new List<EventAttendee>
                {
                    new EventAttendee { Email = appointment.Doctor.Email },
                    new EventAttendee { Email = appointment.Patient.Email }
                },
                Status = "tentative"
            };

            await _service.Events.Insert(calendarEvent, calendarId).ExecuteAsync();
        }

        public async Task ConfirmEvent(AppUser user, Appointment appointment)
        {
            var calendarId = user.CalendarId;
            var eventId = "event_" + appointment.AppointmentId.ToString();
            var calendarEvent = await _service.Events.Get(calendarId, eventId).ExecuteAsync();

            if (calendarEvent == null)
            {
                throw new Exception("Event not found");
            }

            calendarEvent.Status = "confirmed";

            await _service.Events.Update(calendarEvent, calendarId, eventId).ExecuteAsync();
        }

        public async Task CancelEvent(AppUser user, Appointment appointment)
        {
            var calendarId = user.CalendarId;
            var eventId = "event" + appointment.AppointmentId.ToString();
            var calendarEvent = await _service.Events.Get(calendarId, eventId).ExecuteAsync();
            
            if (calendarEvent == null)
            {
                throw new Exception("Event not found");
            }

            calendarEvent.Status = "cancelled";

            await _service.Events.Update(calendarEvent, calendarId, eventId).ExecuteAsync();
        }

        public async Task DeleteCalendar(AppUser user)
        {
            var calendarId = user.CalendarId;
            await _service.Calendars.Delete(calendarId).ExecuteAsync();
        }
    }
}
