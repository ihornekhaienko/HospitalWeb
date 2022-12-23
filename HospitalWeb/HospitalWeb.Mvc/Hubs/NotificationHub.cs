using HospitalWeb.Domain.Entities;
using HospitalWeb.Mvc.Clients.Implementations;
using HospitalWeb.Mvc.Models.ResourceModels;
using Microsoft.AspNetCore.SignalR;

namespace HospitalWeb.Mvc.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly ApiUnitOfWork _api;

        public NotificationHub(ApiUnitOfWork api)
        {
            _api = api;
        }

        public async Task NotifySignUp(string receiver, string topic, string message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(receiver))
                {
                    throw new ArgumentNullException(nameof(receiver));
                }

                var notification = new NotificationResourceModel
                {
                    Topic = topic,
                    Message = message,
                    IsRead = false,
                    AppUserId = receiver,
                    Type = NotificationType.Primary
                };
                _api.Notifications.Post(notification);

                await Clients.User(receiver).SendAsync("NotifySignUp", topic, message);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                throw;
            }
        }

        public async Task NotifyCancel(string receiver, string topic, string message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(receiver))
                {
                    throw new ArgumentNullException(nameof(receiver));
                }

                var notification = new NotificationResourceModel
                {
                    Topic = topic,
                    Message = message,
                    IsRead = false,
                    AppUserId = receiver,
                    Type = NotificationType.Danger
                };
                _api.Notifications.Post(notification);

                await Clients.User(receiver).SendAsync("NotifyCancel", topic, message);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                throw;
            }
        }

        public async Task NotifyFill(string receiver, string topic, string message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(receiver))
                {
                    throw new ArgumentNullException(nameof(receiver));
                }

                var notification = new NotificationResourceModel
                {
                    Topic = topic,
                    Message = message,
                    IsRead = false,
                    AppUserId = receiver,
                    Type = NotificationType.Success
                };
                _api.Notifications.Post(notification);

                await Clients.User(receiver).SendAsync("NotifyFill", topic, message);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                throw;
            }
        }
    }
}
