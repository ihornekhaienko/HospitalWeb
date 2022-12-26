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

        public async Task SendMessageToAdmins(string user, string message, DateTime datetime)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user))
                {
                    throw new ArgumentNullException(nameof(user));
                }

                var messageResource = new MessageResourceModel
                {
                    Text = message,
                    UserId = user,
                    DateTime = datetime.AddHours(2),
                    MessageType = MessageType.UserMessage
                };

                var response = _api.Messages.Post(messageResource);

                await Clients.Group("Admin").SendAsync("SendMessageToAdmins",user, message, datetime);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }

        public async Task SendMessageToUser(string user, string message, DateTime datetime)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user))
                {
                    throw new ArgumentNullException(nameof(user));
                }

                var messageResource = new MessageResourceModel
                {
                    Text = message,
                    UserId = user,
                    DateTime = datetime,
                    MessageType = MessageType.AdminMessage
                };

                var response = _api.Messages.Post(messageResource);

                await Clients.Group("Admin").SendAsync("SendMessageToUser", user, message, datetime);
                await Clients.User(user).SendAsync("SendMessageToUser", user, message, datetime);
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
