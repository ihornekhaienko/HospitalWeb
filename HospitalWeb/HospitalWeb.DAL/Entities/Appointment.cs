using HospitalWeb.DAL.Entities.Identity;

namespace HospitalWeb.DAL.Entities
{
    public enum State
    {
        Planned,
        Active,
        Completed,
        Canceled,
        Missed
    }

    public class Appointment
    {
        public Appointment()
        {
            State = State.Active;
        }

        public int AppointmentId { get; set; }
        public string Diagnosis { get; set; }
        public string Prescription { get; set; }
        public DateTime AppointmentDate { get; set; }
        public State State { get; set; }
        
        public Doctor Doctor { get; set; }
        public Patient Patient { get; set; }
    }
}
