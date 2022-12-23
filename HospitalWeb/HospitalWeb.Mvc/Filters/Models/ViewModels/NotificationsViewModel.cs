using HospitalWeb.Mvc.Filters.Models.DTO;

namespace HospitalWeb.Mvc.Filters.Models.ViewModels
{
    public class NotificationsViewModel
    {
        public IEnumerable<NotificationDTO> Notifications { get; set; }

        public PageModel PageModel { get; set; }
    }
}
