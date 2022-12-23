namespace HospitalWeb.Domain.Entities
{
    public class Meeting
    {
        public int MeetingId { get; set; }

        public string Topic { get; set; }

        public int Duration { get; set; }

        public string StartLink { get; set; }

        public string JoinLink { get; set; }

        public int AppointmentId { get; set; }

        public Appointment Appointment { get; set; }
    }
}
