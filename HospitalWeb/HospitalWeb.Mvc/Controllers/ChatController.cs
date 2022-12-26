using HospitalWeb.Mvc.Clients.Implementations;
using HospitalWeb.Mvc.ViewModels.Chat;
using HospitalWeb.Mvc.ViewModels.Error;
using Microsoft.AspNetCore.Mvc;

namespace HospitalWeb.Mvc.Controllers
{
    public class ChatController : Controller
    {
        private readonly ILogger<ChatController> _logger;
        private readonly ApiUnitOfWork _api;

        public ChatController(ILogger<ChatController> logger, ApiUnitOfWork api)
        {
            _logger = logger;
            _api = api;
        }

        [HttpPost]
        public IActionResult GetChats()
        {
            try
            {
                var response = _api.Messages.Get();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(_api.Messages.ReadError<string>(response));
                }

                var messages = _api.Messages.ReadMany(response);
                var chats = messages
                    .GroupBy(m => m.UserId)
                    .Select(g => g.OrderByDescending(m => m.DateTime).FirstOrDefault())
                    .Select(m => new ChatsViewModel
                    {
                        UserId = m.UserId,
                        FullName = m.User.ToString(),
                        LastMessageDateTime = m.DateTime,
                        LastMessage = m.Text
                    });

                return Json(chats);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in ChatController.GetChats: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
        }

        [HttpPost]
        public IActionResult GetUserMessages([FromBody] string userId)
        {
            try
            {
                var response = _api.Messages.Filter(userId);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(_api.Messages.ReadError<string>(response));
                }

                var messages = _api.Messages.ReadMany(response)
                    .OrderBy(m => m.DateTime)
                    .Select(m => new MessageViewModel { Text = m.Text, DateTime = m.DateTime, MessageType = m.MessageType, UserId = m.UserId });

                return Json(messages);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in ChatController.GetUserMessages: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
        }

    }
}
