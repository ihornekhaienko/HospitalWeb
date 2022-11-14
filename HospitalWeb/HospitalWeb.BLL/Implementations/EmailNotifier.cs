using HospitalWeb.Services.Interfaces;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Configuration;
using System.Collections.Specialized;
using Microsoft.Extensions.Configuration;

namespace HospitalWeb.Services.Implementations
{
    internal class EmailNotifier : INotifier
    {
        private readonly ILogger<EmailNotifier> _logger;
        private readonly IConfiguration _config;

        public EmailNotifier(ILogger<EmailNotifier> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task<bool> NotifyAdd(string receiver, string username, string password)
        {
            return await SendMessage(receiver, 
                "Your clinic account", 
                $"<h4>An account has been created for the clinic:</h4><p>Username: {username}</p><p>Password: {password}</p>");
        }

        public async Task<bool> NotifyDelete(string receiver, string username)
        {
            return await SendMessage(receiver,
                "Your clinic account", 
                $"<p>Your clinic account ({username}) has been deleted</p>");
        }

        public async Task<bool> NotifyUpdate(string receiver, string username)
        {
            return await SendMessage(receiver, 
                "Your clinic account", 
                $"<p>Your clinic account ({username}) has been updated</p>");
        }

        public async Task<bool> SendConfirmationLink(string receiver, string confirmationLink)
        {
            return await SendMessage(receiver,
                "Confirm your email",
                $"<a href={confirmationLink}>Confirmation link</a>");
        }

        public async Task<bool> SendMessage(string receiver, string subject, string message)
        {
            var apiKey = _config["Sendgrid:Key"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_config["Sendgrid:Email"]);
            var to = new EmailAddress(receiver);
            var plainTextContent = message;
            var htmlContent = $"<p>{message}</p>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

            return response.IsSuccessStatusCode;
        }
    }
}
