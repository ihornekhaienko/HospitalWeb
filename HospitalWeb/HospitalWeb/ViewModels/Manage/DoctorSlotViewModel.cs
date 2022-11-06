namespace HospitalWeb.ViewModels.Manage
{
    public class DoctorSlotViewModel
    {
        public string? Doctor { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
