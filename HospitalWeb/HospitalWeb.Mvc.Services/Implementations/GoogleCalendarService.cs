using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using HospitalWeb.Domain.Entities;
using HospitalWeb.Domain.Entities.Identity;
using HospitalWeb.Mvc.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HospitalWeb.Mvc.Services.Implementations
{
    public class GoogleCalendarService : ICalendarService
    {
        private readonly ILogger<GoogleCalendarService> _logger;
        private readonly IConfiguration _config;
        private readonly CalendarService _service;

        public GoogleCalendarService(ILogger<GoogleCalendarService> logger, IConfiguration config)
        {
            _logger = logger;
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

        public async Task<string> CreateCalendar(string email)
        {
            var rule = new AclRule
            {
                Scope = new AclRule.ScopeData
                {
                    Type = "user",
                    Value = email
                },
                Role = "reader"
            };

            var calendar = new Calendar()
            {
                Summary = "Appointments",
                Description = "Your appointments in HospitalDb"
            };

            var calendarId = (await _service.Calendars.Insert(calendar).ExecuteAsync()).Id;
            await _service.Acl.Insert(rule, calendarId).ExecuteAsync();

            return calendarId;
        }

        public string GetCalendar(AppUser user)
        {
            if (string.IsNullOrEmpty(user.CalendarId))
            {
                return null;
            }

            return _config["Calendar:CalendarUrl"] + user.CalendarId + _config["Calendar:TimeZone"];
        }

        public async Task CreateEvent(AppUser user, Appointment appointment)
        {
            var calendarId = user.CalendarId;

            if (string.IsNullOrEmpty(calendarId))
            {
                return;
            }

            var calendarEvent = new Event()
            {
                Id = "event" + appointment.AppointmentId.ToString(),
                Summary = $"Appointment on {appointment.AppointmentDate.ToString("MM/dd/yyyy HH:mm")}",
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
                Status = "tentative"
            };

            await _service.Events.Insert(calendarEvent, calendarId).ExecuteAsync();
        }

        public async Task ConfirmEvent(AppUser user, Appointment appointment)
        {
            var calendarId = user.CalendarId;

            if (string.IsNullOrEmpty(calendarId))
            {
                return;
            }

            var eventId = "event" + appointment.AppointmentId.ToString();
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

            if (string.IsNullOrEmpty(calendarId))
            {
                return;
            }

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

            if (string.IsNullOrEmpty(calendarId))
            {
                return;
            }

            await _service.Calendars.Delete(calendarId).ExecuteAsync();
        }
    }
}
