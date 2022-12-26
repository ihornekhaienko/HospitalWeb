namespace HospitalWeb.Mvc.ViewModels.Chat
{
    public class ChatsViewModel
    {
        public string UserId { get; set; }

        public string FullName { get; set; }

        public DateTime LastMessageDateTime { get; set; }   

        public string LastMessage { get; set; }
    }
}
