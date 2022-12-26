using HospitalWeb.Domain.Entities;

namespace HospitalWeb.Mvc.ViewModels.Chat
{
    public class MessageViewModel
    {
        public string Text { get; set; }

        public DateTime DateTime { get; set; }

        public MessageType MessageType { get; set; }

        public string UserId { get; set; }
    }
}
