namespace HospitalWeb.WebApi.Models.ResourceModels
{
    public class MessageResourceModel
    {
        public int MessageId { get; set; }

        public string Text { get; set; }

        public DateTime DateTime { get; set; }

        public string SenderId { get; set; }

        public string ReceiverId { get; set; }
    }
}
