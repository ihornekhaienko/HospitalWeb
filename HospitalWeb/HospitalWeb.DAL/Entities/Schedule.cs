using HospitalWeb.DAL.Entities.Identity;

namespace HospitalWeb.DAL.Entities
{
    public class Schedule
    {
        public int ScheduleId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        
        public Doctor? Doctor { get; set; }
    }
}
