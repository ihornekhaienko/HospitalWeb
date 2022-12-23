namespace HospitalWeb.Domain.Entities
{
    public class Diagnosis
    {
        public Diagnosis()
        {
            Appointments = new List<Appointment>();
        }

        public int DiagnosisId { get; set; }

        public string DiagnosisName { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
    }
}
