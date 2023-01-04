using System.ComponentModel.DataAnnotations;

namespace HospitalWeb.Mvc.ViewModels.Administration
{
    public class DoctorSlotViewModel
    {
        public string DoctorId { get; set; }

        public string DoctorFullName { get; set; }

        public int? ScheduleId { get; set; }

        [Required(ErrorMessage = "DayOfWeekRequired")]
        [Display(Name = "DaySelect")]
        public string DayOfWeek { get; set; }

        [Required(ErrorMessage = "StartTimeRequired")]
        [TimeLessThan("EndTime", ErrorMessage = "StartTimeSelectError")]
        [Display(Name = "StartTimeSelect")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "EndTimeRequired")]
        [Display(Name = "EndTimeSelect")]
        public DateTime EndTime { get; set; }
    }
}
