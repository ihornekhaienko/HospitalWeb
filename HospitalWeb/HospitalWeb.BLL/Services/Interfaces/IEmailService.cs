namespace HospitalWeb.BLL.Services.Interfaces
{
    public interface INotifier
    {
        public void NotifyAdd(string receiver, string username, string password);
        public void NotifyDelete(string receiver, string username);
        public void NotifyUpdate(string receiver, string username);
        public void SendMessage(string receiver, string subject, string message);
    }
}
