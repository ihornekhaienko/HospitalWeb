namespace HospitalWeb.Mvc.Services.Interfaces
{
    public interface IEmailService
    {
        public Task NotifyAdd(string receiver, string username, string password);
        public Task NotifyDelete(string receiver, string username);
        public Task NotifyUpdate(string receiver, string username);
        public Task SendConfirmationLink(string receiver, string confirmationLink);
        public Task SendResetPasswordLink(string receiver, string resetPasswordLink);
        public Task SendMessage(string receiver, string subject, string message);
    }
}
