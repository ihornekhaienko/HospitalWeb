using HospitalWeb.Domain.Entities;

namespace HospitalWeb.WebApi.Models.ResourceModels
{
    public class MessageResourceModel
    {
        public int MessageId { get; set; }

        public string Text { get; set; }

        public DateTime DateTime { get; set; }

        public MessageType MessageType { get; set; }

        public string UserId { get; set; }
    }
}
