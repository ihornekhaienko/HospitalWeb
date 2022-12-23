using HospitalWeb.Domain.Entities;

namespace HospitalWeb.Mvc.Services.Interfaces
{
    public interface IMeetingService
    {
        public Meeting CreateMeeting(Appointment appointment);
    }
}
