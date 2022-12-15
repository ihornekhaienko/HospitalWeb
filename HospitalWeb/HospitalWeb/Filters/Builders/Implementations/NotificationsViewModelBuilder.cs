using HospitalWeb.Filters.Builders.Interfaces;
using HospitalWeb.Filters.Models;
using HospitalWeb.Filters.Models.DTO;
using HospitalWeb.Filters.Models.ViewModels;
using HospitalWeb.Clients.Implementations;

namespace HospitalWeb.Filters.Builders.Implementations
{
    public class NotificationsViewModelBuilder : ViewModelBuilder<NotificationsViewModel>
    {
        private readonly ApiUnitOfWork _api;
        private IEnumerable<NotificationDTO> _notifications;
        private PageModel _pageModel;
        private int _count = 0;

        public NotificationsViewModelBuilder(
            ApiUnitOfWork api,
            int pageNumber,
            string searchString,
            int pageSize = 10
        ) : base(pageNumber, pageSize, searchString)
        {
            _api = api;
        }

        public override void BuildEntityModel()
        {
            var response = _api.Notifications.Filter(_searchString, _pageSize, _pageNumber);

            if (response.IsSuccessStatusCode)
            {
                _notifications = _api.Notifications.ReadMany(response)
                    .Select(n => new NotificationDTO
                    {
                        NotificationId = n.NotificationId,
                        Topic = n.Topic,
                        Message = n.Message,
                        IsRead = n.IsRead,
                        Type = n.Type
                    });

                _count = Convert.ToInt32(response.Headers.GetValues("TotalCount").FirstOrDefault());
            }
            else
            {
                throw new Exception("Failed loading notifications");
            }
        }

        public override void BuildFilterModel()
        {
        }

        public override void BuildPageModel()
        {
            _pageModel = new PageModel(_count, _pageNumber, _pageSize);
        }

        public override void BuildSortModel()
        {
        }

        public override NotificationsViewModel GetViewModel()
        {
            if (_pageModel == null || _notifications == null)
            {
                throw new Exception("Failed building view model, some of models is null");
            }

            return new NotificationsViewModel
            {
                PageModel = _pageModel,
                Notifications = _notifications
            };
        }
    }
}
