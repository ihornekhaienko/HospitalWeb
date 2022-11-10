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
    public class PatientsViewModelBuilder : ViewModelBuilder<PatientsViewModel>
    {
        private readonly UnitOfWork _uow;
        private readonly int? _locality;
        private readonly PatientSortState _sortOrder;
        private IEnumerable<PatientDTO> _patients;
        private PageModel _pageModel;
        private PatientFilterModel _filterModel;
        private PatientSortModel _sortModel;
        private int _count = 0;

        public PatientsViewModelBuilder(
            UnitOfWork uow,
            int pageNumber,
            string searchString,
            PatientSortState sortOrder,
            int? locality = null,
            int pageSize = 10
            ) : base(pageNumber, pageSize, searchString)
        {
            _uow = uow;
            _sortOrder = sortOrder;
            _locality = locality;
        }

        public override void BuildEntityModel()
        {
            Func<Patient, bool> filter = (p) =>
            {
                bool result = true;

                if (!string.IsNullOrWhiteSpace(_searchString))
                {
                    result = p.Name.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ||
                    p.Surname.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ||
                    p.Email.Contains(_searchString, StringComparison.OrdinalIgnoreCase);
                }

                if (_locality != null && _locality != 0)
                {
                    result = result && p.Address.Locality.LocalityId == _locality;
                }

                return result;
            };

            Func<IQueryable<Patient>, IOrderedQueryable<Patient>> orderBy = (patients) =>
            {
                _count = patients.Count();

                switch (_sortModel.Current)
                {
                    case PatientSortState.NameAsc:
                        patients = patients.OrderBy(p => p.Name);
                        break;
                    case PatientSortState.NameDesc:
                        patients = patients.OrderByDescending(p => p.Name);
                        break;
                    case PatientSortState.SurnameAsc:
                        patients = patients.OrderBy(p => p.Surname);
                        break;
                    case PatientSortState.SurnameDesc:
                        patients = patients.OrderByDescending(p => p.Surname);
                        break;
                    case PatientSortState.EmailAsc:
                        patients = patients.OrderBy(p => p.Email);
                        break;
                    case PatientSortState.EmailDesc:
                        patients = patients.OrderByDescending(p => p.Email);
                        break;
                    case PatientSortState.PhoneAsc:
                        patients = patients.OrderBy(p => p.PhoneNumber);
                        break;
                    case PatientSortState.PhoneDesc:
                        patients = patients.OrderByDescending(p => p.PhoneNumber);
                        break;
                    case PatientSortState.AddressAsc:
                        patients = patients
                            .OrderBy(p => p.Address.FullAddress)
                            .ThenBy(a => a.Address.Locality.LocalityName);
                        break;
                    case PatientSortState.AddressDesc:
                        patients = patients
                            .OrderByDescending(p => p.Address.FullAddress)
                            .ThenByDescending(a => a.Address.Locality.LocalityName);
                        break;
                    default:
                        patients = patients.OrderBy(d => d.Id);
                        break;
                }

                return  (IOrderedQueryable<Patient>)patients;
            };

            _patients = _uow.Patients
                .GetAll(filter: filter, orderBy: orderBy, first: _pageSize, offset: (_pageNumber - 1) * _pageSize)
                .Select(p => new PatientDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Surname = p.Surname,
                    Email = p.Email,
                    PhoneNumber = p.PhoneNumber,
                    Image = p.Image,
                    Address = p.Address.ToString()
                });
        }

        public override void BuildFilterModel()
        {
            var localities = _uow.Localities
                .GetAll()
                .OrderBy(l => l.LocalityName)
                .Select(l => new LocalityDTO { LocalityId = l.LocalityId, LocalityName = l.LocalityName })
                .ToList();

            _filterModel = new PatientFilterModel(_searchString, localities, _locality);
        }

        public override void BuildPageModel()
        {
            _pageModel = new PageModel(_count, _pageNumber, _pageSize);
        }

        public override void BuildSortModel()
        {
            _sortModel = new PatientSortModel(_sortOrder);
        }

        public override PatientsViewModel GetViewModel()
        {
            if (_pageModel == null || _sortModel == null || _filterModel == null)
            {
                throw new Exception("Failed building view model, some of models is null");
            }

            return new PatientsViewModel
            {
                Patients = _patients,
                PageModel = _pageModel,
                SortModel = _sortModel,
                FilterModel = _filterModel
            };
        }
    }
}
