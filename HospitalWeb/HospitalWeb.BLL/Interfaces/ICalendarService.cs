using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Entities.Identity;

namespace HospitalWeb.Services.Interfaces
{
    public interface ICalendarService
    {
        public Task CreateCalendar(AppUser user);
        public string GetCalendar(AppUser user);
        public Task DeleteCalendar(AppUser user);
        public Task CreateEvent(AppUser user, Appointment appointment);
        public Task ConfirmEvent(AppUser user, Appointment appointment);
        public Task CancelEvent(AppUser user, Appointment appointment);
    }
}
