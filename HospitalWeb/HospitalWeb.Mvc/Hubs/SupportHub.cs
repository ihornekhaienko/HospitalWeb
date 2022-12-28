using HospitalWeb.Domain.Entities;
using HospitalWeb.Mvc.Clients.Implementations;
using HospitalWeb.Mvc.Models.ResourceModels;
using Microsoft.AspNetCore.SignalR;

namespace HospitalWeb.Mvc.Hubs
{
    public class SupportHub : Hub
    {
        private readonly ApiUnitOfWork _api;

        public SupportHub(ApiUnitOfWork api)
        {
            _api = api;
        }

        public async Task SendMessageToAdmins(string userId, string fullName, string message, DateTime datetime)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    throw new ArgumentNullException(nameof(userId));
                }

                var messageResource = new MessageResourceModel
                {
                    Text = message,
                    UserId = userId,
                    DateTime = datetime.AddHours(2),
                    MessageType = MessageType.UserMessage
                };

                var response = _api.Messages.Post(messageResource);

                await Clients.Group("Admin").SendAsync("SendMessageToAdmins", userId, fullName, message, datetime);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }

        public async Task SendMessageToUser(string userId, string fullName, string message, DateTime datetime)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    throw new ArgumentNullException(nameof(userId));
                }

                var messageResource = new MessageResourceModel
                {
                    Text = message,
                    UserId = userId,
                    DateTime = datetime,
                    MessageType = MessageType.AdminMessage
                };

                var response = _api.Messages.Post(messageResource);

                await Clients.Group("Admin").SendAsync("SendMessageToUser", userId, fullName, message, datetime);
                await Clients.User(userId).SendAsync("SendMessageToUser", userId, fullName, message, datetime);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }

        public async Task SendMessageFromUnauthorized(string message, DateTime datetime)
        {
            try
            {
                string connectionId = Context.ConnectionId;

                await Clients.Group("Admin").SendAsync("SendMessageFromUnauthorized", connectionId, message, datetime);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }

        public async Task SendMessageToUnauthorized(string connectionId, string message, DateTime datetime)
        {
            try
            {
                await Clients.Client(connectionId).SendAsync("SendMessageToUnauthorized", connectionId, message, datetime);
                await Clients.Group("Admin").SendAsync("SendMessageToUnauthorized", connectionId, message, datetime);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }

        public override async Task OnConnectedAsync()
        {
            if (Context.User.IsInRole("Admin"))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admin");
            }

            await base.OnConnectedAsync();
        }
    }
}
