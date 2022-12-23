namespace HospitalWeb.Models.ResourceModels
{
    public class ScheduleResourceModel
    {
        public DayOfWeek DayOfWeek { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string DoctorId { get; set; }
    }
}
