using HospitalWeb.Attributes.Validation;
using System.ComponentModel.DataAnnotations;

namespace HospitalWeb.ViewModels.Manage
{
    public class DoctorSlotViewModel
    {
        public string DoctorId { get; set; }

        public string DoctorFullName { get; set; }

        public int? ScheduleId { get; set; }

        [Required]
        [Display(Name = "Select day")]
        public string DayOfWeek { get; set; }

        [Required]
        [TimeLessThan("EndTime", ErrorMessage = "The start time must lower than the end time")]
        [Display(Name = "Select starting time")]
        public DateTime StartTime { get; set; }

        [Required]
        [Display(Name = "Select ending time")]
        public DateTime EndTime { get; set; }
    }
}
