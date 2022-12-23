using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Implementations;
using HospitalWeb.Filters.Builders.Interfaces;
using HospitalWeb.Filters.Models;
using HospitalWeb.Filters.Models.DTO;
using HospitalWeb.Filters.Models.FilterModels;
using HospitalWeb.Filters.Models.SortModels;
using HospitalWeb.Filters.Models.ViewModels;
using HospitalWeb.Clients.Implementations;
using HospitalWeb.Models.SortStates;

namespace HospitalWeb.Filters.Builders.Implementations
{
    public class AdminsViewModelBuilder : ViewModelBuilder<AdminsViewModel>
    {
        private readonly ApiUnitOfWork _api;
        private readonly AdminSortState _sortOrder;
        private IEnumerable<AdminDTO> _admins;
        private PageModel _pageModel;
        private AdminFilterModel _filterModel;
        private AdminSortModel _sortModel;
        private int _count = 0;

        public AdminsViewModelBuilder(
            ApiUnitOfWork api,
            int pageNumber,
            string searchString,
            AdminSortState sortOrder,
            int pageSize = 10
            ) : base(pageNumber, pageSize, searchString)
        {
            _api = api;
            _sortOrder = sortOrder;
        }

        public override void BuildEntityModel()
        {
            var response = _api.Admins.Filter(_searchString, _sortOrder, _pageSize, _pageNumber);

            if (response.IsSuccessStatusCode)
            {
                _admins = _api.Admins.ReadMany(response)
                    .Select(a => new AdminDTO
                    {
                        Id = a.Id,
                        Email = a.Email,
                        PhoneNumber = a.PhoneNumber,
                        Name = a.Name,
                        Surname = a.Surname,
                        Image = a.Image,
                        IsSuperAdmin = a.IsSuperAdmin
                    });

                _count = Convert.ToInt32(response.Headers.GetValues("TotalCount").FirstOrDefault());
            }
            else
            {
                throw new Exception("Failed loading doctors");
            }
        }

        public override void BuildFilterModel()
        {
            _filterModel = new AdminFilterModel(_searchString);
        }

        public override void BuildPageModel()
        {
            _pageModel = new PageModel(_count, _pageNumber, _pageSize);
        }

        public override void BuildSortModel()
        {
            _sortModel = new AdminSortModel(_sortOrder);
        }

        public override AdminsViewModel GetViewModel()
        {
            if (_pageModel == null || _sortModel == null || _filterModel == null)
            {
                throw new Exception("Failed building view model, some of models is null");
            }

            return new AdminsViewModel
            {
                Admins = _admins,
                PageModel = _pageModel,
                SortModel = _sortModel,
                FilterModel = _filterModel
            };
        }
    }
}
