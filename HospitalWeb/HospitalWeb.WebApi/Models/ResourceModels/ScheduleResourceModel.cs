namespace HospitalWeb.WebApi.Models.ResourceModels
{
    public class ScheduleResourceModel
    {
        public int? ScheduleId { get; set; }

        public DayOfWeek DayOfWeek { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string DoctorId { get; set; }
    }
}
