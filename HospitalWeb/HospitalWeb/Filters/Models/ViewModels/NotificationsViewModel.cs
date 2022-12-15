using HospitalWeb.Filters.Models.DTO;

namespace HospitalWeb.Filters.Models.ViewModels
{
    public class NotificationsViewModel
    {
        public IEnumerable<NotificationDTO> Notifications { get; set; }

        public PageModel PageModel { get; set; }
    }
}
