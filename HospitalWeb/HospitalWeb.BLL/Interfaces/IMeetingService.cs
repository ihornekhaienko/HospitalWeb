using HospitalWeb.Domain.Entities;

namespace HospitalWeb.Services.Interfaces
{
    public interface IMeetingService
    {
        public Meeting CreateMeeting(Appointment appointment);
    }
}
