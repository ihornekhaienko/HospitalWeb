namespace HospitalWeb.ViewModels.Appointments
{
    public class AppointmentViewModel
    {
        public int AppointmentId { get; set; }

        public string Diagnosis { get; set; }

        public string Prescription { get; set; }

        public DateTime AppointmentDate { get; set; }

        public string DoctorId { get; set; }

        public string Doctor { get; set; }

        public string DoctorSpecialty { get; set; }

        public string Patient { get; set; }
    }
}
