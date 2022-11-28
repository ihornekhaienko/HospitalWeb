namespace HospitalWeb.WebApi.Models.ResourceModels
{
    public class MeetingResourceModel
    {
        public int MeetingId { get; set; }

        public string Topic { get; set; }

        public int Duration { get; set; }

        public string StartLink { get; set; }

        public string JoinLink { get; set; }

        public int AppointmentId { get; set; }
    }
}
