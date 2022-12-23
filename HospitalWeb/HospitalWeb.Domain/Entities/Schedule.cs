using HospitalWeb.Domain.Entities.Identity;

namespace HospitalWeb.Domain.Entities
{
    public class Schedule
    {
        public int ScheduleId { get; set; }

        public DayOfWeek DayOfWeek { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
        
        public string DoctorId { get; set; }
        public Doctor Doctor { get; set; }
    }
}
