using HospitalWeb.Domain.Entities;

namespace HospitalWeb.Mvc.Models.ResourceModels
{
    public class AppointmentResourceModel
    {
        public string Prescription { get; set; }

        public DateTime AppointmentDate { get; set; }

        public State State { get; set; }

        public int? DiagnosisId { get; set; }

        public string DoctorId { get; set; }

        public string PatientId { get; set; }

        public decimal Price { get; set; }
    }
}
