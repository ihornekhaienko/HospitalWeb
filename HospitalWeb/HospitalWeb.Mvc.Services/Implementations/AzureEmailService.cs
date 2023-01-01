using Azure.Communication.Email;
using Azure.Communication.Email.Models;
using HospitalWeb.Mvc.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HospitalWeb.Mvc.Services.Implementations
{
    public class AzureEmailService : IEmailService
    {
        private readonly ILogger<AzureEmailService> _logger;
        private readonly string _connectionString;
        private readonly string _sender;

        public AzureEmailService(ILogger<AzureEmailService> logger, IConfiguration config)
        {
            _logger = logger;
            _connectionString = config["EmailService:ConnectionString"];
            _sender = config["EmailService:Email"];
        }

        public async Task NotifyAdd(string receiver, string username, string password)
        {
            await SendMessage(receiver,
                "Your clinic account",
                $"<h4>An account has been created for the clinic:</h4><p>Username: {username}</p><p>Password: {password}</p>");
        }

        public async Task NotifyDelete(string receiver, string username)
        {
            await SendMessage(receiver,
                "Your clinic account",
                $"<p>Your clinic account ({username}) has been deleted</p>");
        }

        public async Task NotifyUpdate(string receiver, string username)
        {
            await SendMessage(receiver,
                "Your clinic account",
                $"<p>Your clinic account ({username}) has been updated</p>");
        }

        public async Task SendConfirmationLink(string receiver, string confirmationLink)
        {
            await SendMessage(receiver,
                "Confirm your email",
                $"<p>Thank you for registering on our Hospital service.</p>" +
                $"<p>Please confirm your email by clicking <a href='{confirmationLink}'>here</a>.</p>");
        }

        public async Task SendResetPasswordLink(string receiver, string resetPasswordLink)
        {
            await SendMessage(receiver,
                "Reset your password",
                $"<p>Please reset your password by clicking <a href='{resetPasswordLink}'>here</a>.</p>");
        }

        public async Task SendMessage(string receiver, string subject, string message)
        {
            var emailClient = new EmailClient(_connectionString);

            var emailContent = new EmailContent(subject);
            emailContent.Html = $"<div>{message}</div>";

            var emailRecipients = new EmailRecipients(
                new List<EmailAddress>
                {
                    new EmailAddress(email: receiver)
                });

            var emailMessage = new EmailMessage(_sender, emailContent, emailRecipients);

            try
            {
                var response = await emailClient.SendAsync(emailMessage);
            }
            catch (Exception err)
            {
                _logger.LogError(err.Message);
                _logger.LogTrace(err.StackTrace);
            }
        }
    }
}
