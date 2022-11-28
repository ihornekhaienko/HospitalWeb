using HospitalWeb.DAL.Entities;

namespace HospitalWeb.Services.Interfaces
{
    public interface IMeetingService
    {
        public Meeting CreateMeeting(Appointment appointment);
    }
}
