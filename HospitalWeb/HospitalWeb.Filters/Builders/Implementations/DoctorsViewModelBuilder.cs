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
    public class DoctorsViewModelBuilder : ViewModelBuilder<DoctorsViewModel>
    {
        private readonly UnitOfWork _uow;
        private readonly int? _specialty;
        private readonly DoctorSortState _sortOrder;
        private IEnumerable<DoctorDTO> _doctors;
        private PageModel _pageModel;
        private DoctorFilterModel _filterModel;
        private DoctorSortModel _sortModel;
        private int _count = 0;

        public DoctorsViewModelBuilder(
            UnitOfWork uow, 
            int pageNumber, 
            string searchString,
            DoctorSortState sortOrder, 
            int? specialty = null, 
            int pageSize = 10
            ) : base(pageNumber, pageSize, searchString)
        {
            _uow = uow;
            _sortOrder = sortOrder;
            _specialty = specialty;
        }

        public override void BuildEntityModel()
        {
            Func<Doctor, bool> filter = (d) =>
            {
                bool result = true;

                if (!string.IsNullOrWhiteSpace(_searchString))
                {
                    result = d.Name.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ||
                    d.Surname.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ||
                    d.Email.Contains(_searchString, StringComparison.OrdinalIgnoreCase);
                }

                if (_specialty != null && _specialty != 0)
                {
                    result = result && d.Specialty.SpecialtyId == _specialty;
                }

                return result;
            };

            Func<IQueryable<Doctor>, IOrderedQueryable<Doctor>> orderBy = (doctors) =>
            {
                _count = doctors.Count();

                switch (_sortModel.Current)
                {
                    case DoctorSortState.NameAsc:
                        doctors = doctors.OrderBy(d => d.Name);
                        break;
                    case DoctorSortState.NameDesc:
                        doctors = doctors.OrderByDescending(d => d.Name);
                        break;
                    case DoctorSortState.SurnameAsc:
                        doctors = doctors.OrderBy(d => d.Surname);
                        break;
                    case DoctorSortState.SurnameDesc:
                        doctors = doctors.OrderByDescending(d => d.Surname);
                        break;
                    case DoctorSortState.EmailAsc:
                        doctors = doctors.OrderBy(d => d.Email);
                        break;
                    case DoctorSortState.EmailDesc:
                        doctors = doctors.OrderByDescending(d => d.Email);
                        break;
                    case DoctorSortState.PhoneAsc:
                        doctors = doctors.OrderBy(d => d.PhoneNumber);
                        break;
                    case DoctorSortState.PhoneDesc:
                        doctors = doctors.OrderByDescending(d => d.PhoneNumber);
                        break;
                    case DoctorSortState.SpecialtyAsc:
                        doctors = doctors.OrderBy(d => d.Specialty.SpecialtyName);
                        break;
                    case DoctorSortState.SpecialtyDesc:
                        doctors = doctors.OrderByDescending(d => d.Specialty.SpecialtyName);
                        break;
                    default:
                        doctors = doctors.OrderBy(d => d.Id);
                        break;
                }

                return (IOrderedQueryable<Doctor>)doctors;
            };

            _doctors = _uow.Doctors
                .GetAll(filter: filter, orderBy: orderBy, first: _pageSize, offset: (_pageNumber - 1) * _pageSize)
                .Select(d => new DoctorDTO
                {
                    Id = d.Id,
                    Name = d.Name,
                    Surname = d.Surname,
                    Email = d.Email,
                    PhoneNumber = d.PhoneNumber,
                    Image = d.Image,
                    Specialty = d.Specialty.SpecialtyName
                });
        }

        public override void BuildFilterModel()
        {
            var specialties = _uow.Specialties
                .GetAll()
                .OrderBy(s => s.SpecialtyName)
                .Select(s => new SpecialtyDTO { SpecialtyId = s.SpecialtyId, SpecialtyName = s.SpecialtyName })
                .ToList();

            _filterModel = new DoctorFilterModel(_searchString, specialties, _specialty);
        }

        public override void BuildPageModel()
        {
            _pageModel = new PageModel(_count, _pageNumber, _pageSize);
        }

        public override void BuildSortModel()
        {
            _sortModel = new DoctorSortModel(_sortOrder);
        }

        public override DoctorsViewModel GetViewModel()
        {
            if (_pageModel == null || _sortModel == null || _filterModel == null)
            {
                throw new Exception("Failed building view model, some of models is null");
            }

            return new DoctorsViewModel
            {
                Doctors = _doctors,
                PageModel = _pageModel,
                SortModel = _sortModel,
                FilterModel = _filterModel
            };
        }
    }
}
