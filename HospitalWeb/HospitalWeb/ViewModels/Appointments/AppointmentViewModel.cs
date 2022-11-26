using System.ComponentModel.DataAnnotations;

namespace HospitalWeb.ViewModels.Appointments
{
    public class AppointmentViewModel
    {
        [Required]
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "DiagnosisRequired")]
        public string Diagnosis { get; set; }

        public string Prescription { get; set; }

        public DateTime AppointmentDate { get; set; }

        public string DoctorId { get; set; }

        public string Doctor { get; set; }

        public string DoctorSpecialty { get; set; }

        public string PatientId { get; set; }

        public string Patient { get; set; }
    }
}
