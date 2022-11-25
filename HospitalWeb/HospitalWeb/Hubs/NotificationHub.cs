using Microsoft.AspNetCore.SignalR;

namespace HospitalWeb.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task NotifySignUp(string receiver, string topic, string message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(receiver))
                {
                    throw new ArgumentNullException(nameof(receiver));
                }

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

                await Clients.User(receiver).SendAsync("NotifyCancel", topic, message);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                throw;
            }
        }
    }
}
