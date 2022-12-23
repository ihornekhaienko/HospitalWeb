namespace HospitalWeb.Mvc.Services.Interfaces
{
    public interface INotifier
    {
        public Task<bool> NotifyAdd(string receiver, string username, string password);
        public Task<bool> NotifyDelete(string receiver, string username);
        public Task<bool> NotifyUpdate(string receiver, string username);
        public Task<bool> SendConfirmationLink(string receiver, string confirmationLink);
        public Task<bool> SendResetPasswordLink(string receiver, string resetPasswordLink);
        public Task<bool> SendMessage(string receiver, string subject, string message);
    }
}
