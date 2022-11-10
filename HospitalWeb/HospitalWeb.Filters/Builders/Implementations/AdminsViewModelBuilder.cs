using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Implementations;
using HospitalWeb.Filters.Builders.Interfaces;
using HospitalWeb.Filters.Models;
using HospitalWeb.Filters.Models.DTO;
using HospitalWeb.Filters.Models.FilterModels;
using HospitalWeb.Filters.Models.SortModels;
using HospitalWeb.Filters.Models.SortStates;
using HospitalWeb.Filters.Models.ViewModels;

namespace HospitalWeb.Filters.Builders.Implementations
{
    public class AdminsViewModelBuilder : ViewModelBuilder<AdminsViewModel>
    {
        private readonly UnitOfWork _uow;
        private readonly AdminSortState _sortOrder;
        private IEnumerable<AdminDTO> _admins;
        private PageModel _pageModel;
        private AdminFilterModel _filterModel;
        private AdminSortModel _sortModel;
        private int _count = 0;

        public AdminsViewModelBuilder(
            UnitOfWork uow,
            int pageNumber,
            string searchString,
            AdminSortState sortOrder,
            int pageSize = 10
            ) : base(pageNumber, pageSize, searchString)
        {
            _uow = uow;
            _sortOrder = sortOrder;
        }

        public override void BuildEntityModel()
        {
            Func<Admin, bool> filter = (a) =>
            {
                bool result = true;

                if (!string.IsNullOrWhiteSpace(_searchString))
                {
                    result = a.Name.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ||
                    a.Surname.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ||
                    a.Email.Contains(_searchString, StringComparison.OrdinalIgnoreCase);
                }

                return result;
            };

            Func<IQueryable<Admin>, IOrderedQueryable<Admin>> orderBy = (admins) =>
            {
                _count = admins.Count();

                switch (_sortModel.Current)
                {
                    case AdminSortState.NameAsc:
                        admins = admins.OrderBy(a => a.Name);
                        break;
                    case AdminSortState.NameDesc:
                        admins = admins.OrderByDescending(a => a.Name);
                        break;
                    case AdminSortState.SurnameAsc:
                        admins = admins.OrderBy(a => a.Surname);
                        break;
                    case AdminSortState.SurnameDesc:
                        admins = admins.OrderByDescending(a => a.Surname);
                        break;
                    case AdminSortState.EmailAsc:
                        admins = admins.OrderBy(a => a.Email);
                        break;
                    case AdminSortState.EmailDesc:
                        admins = admins.OrderByDescending(a => a.Email);
                        break;
                    case AdminSortState.PhoneAsc:
                        admins = admins.OrderBy(a => a.PhoneNumber);
                        break;
                    case AdminSortState.PhoneDesc:
                        admins = admins.OrderByDescending(a => a.PhoneNumber);
                        break;
                    case AdminSortState.LevelAsc:
                        admins = admins.OrderBy(a => a.IsSuperAdmin);
                        break;
                    case AdminSortState.LevelDesc:
                        admins = admins.OrderByDescending(a => a.IsSuperAdmin);
                        break;
                    default:
                        admins = admins.OrderBy(a => a.Id);
                        break;
                }

                return (IOrderedQueryable<Admin>)admins;
            };

            _admins = _uow.Admins
                .GetAll(filter: filter, orderBy: orderBy, first: _pageSize, offset: (_pageNumber - 1) * _pageSize)
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
