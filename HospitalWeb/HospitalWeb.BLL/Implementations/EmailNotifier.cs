using HospitalWeb.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace HospitalWeb.Services.Implementations
{
    public class EmailNotifier : INotifier
    {
        private readonly ILogger<EmailNotifier> _logger;

        public EmailNotifier(ILogger<EmailNotifier> logger)
        {
            _logger = logger;
        }

        public void NotifyAdd(string receiver, string username, string password)
        {
            SendMessage(receiver, "Your clinic account", $"<h4>An account has been created for the clinic:</h4><p>Username: {username}</p><p>Password: {password}</p>");
        }

        public void NotifyDelete(string receiver, string username)
        {
            SendMessage(receiver, "Your clinic account", $"<p>Your clinic account ({username}) has been deleted</p>");
        }

        public void NotifyUpdate(string receiver, string username)
        {
            SendMessage(receiver, "Your clinic account", $"<p>Your clinic account ({username}) has been updated</p>");
        }

        public void SendMessage(string receiver, string subject, string message)
        {
            try
            {
                MailAddress from = new MailAddress("hospital_0711@ukr.net", "Hospital");
                MailAddress to = new MailAddress(receiver);
                MailMessage mailMessage = new MailMessage(from, to)
                {
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };

                SmtpClient smtp = new SmtpClient("smtp.ukr.net", 465)
                {
                    Credentials = new NetworkCredential("hospital_0711@ukr.net", "pass_0711"),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };
                smtp.Send(mailMessage);
            }
            catch (Exception err)
            {
                _logger.LogCritical(err.Message);
            }
        }
    }
}
