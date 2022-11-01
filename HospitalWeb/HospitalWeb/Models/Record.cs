using HospitalWeb.Models.Identity;

namespace HospitalWeb.Models
{
    public enum State
    {
        Planned,
        Active,
        Completed,
        Canceled,
        Missed
    }

    public class Record
    {
        public Record()
        {
            State = State.Active;
        }

        public int RecordId { get; set; }
        public string Diagnosis { get; set; }
        public string Prescription { get; set; }
        public DateTime RecordDate { get; set; }
        public State State { get; set; }
        
        public Doctor Doctor { get; set; }
        public Patient Patient { get; set; }
    }
}
