using HospitalWeb.Domain.Entities.Identity;
using HospitalWeb.Domain.Services.Implementations;
using HospitalWeb.Mvc.Filters.Builders.Interfaces;
using HospitalWeb.Mvc.Filters.Models;
using HospitalWeb.Mvc.Filters.Models.DTO;
using HospitalWeb.Mvc.Filters.Models.FilterModels;
using HospitalWeb.Mvc.Filters.Models.SortModels;
using HospitalWeb.Mvc.Filters.Models.ViewModels;
using HospitalWeb.Mvc.Clients.Implementations;
using HospitalWeb.Mvc.Models.SortStates;

namespace HospitalWeb.Mvc.Filters.Builders.Implementations
{
    public class PatientsViewModelBuilder : ViewModelBuilder<PatientsViewModel>
    {
        private readonly ApiUnitOfWork _api;
        private readonly int? _locality;
        private readonly PatientSortState _sortOrder;
        private IEnumerable<PatientDTO> _patients;
        private PageModel _pageModel;
        private PatientFilterModel _filterModel;
        private PatientSortModel _sortModel;
        private int _count = 0;

        public PatientsViewModelBuilder(
            ApiUnitOfWork api,
            int pageNumber,
            string searchString,
            PatientSortState sortOrder,
            int? locality = null,
            int pageSize = 10
            ) : base(pageNumber, pageSize, searchString)
        {
            _api = api;
            _sortOrder = sortOrder;
            _locality = locality;
        }

        public override void BuildEntityModel()
        {
            var response = _api.Patients.Filter(_searchString, _locality, _sortOrder, _pageSize, _pageNumber);

            if (response.IsSuccessStatusCode)
            {
                _patients = _api.Patients.ReadMany(response)
                    .Select(p => new PatientDTO
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Surname = p.Surname,
                        Email = p.Email,
                        PhoneNumber = p.PhoneNumber,
                        Image = p.Image,
                        BirthDate = p.BirthDate,
                        Address = p.Address.ToString()
                    });

                _count = Convert.ToInt32(response.Headers.GetValues("TotalCount").FirstOrDefault());
            }
            else
            {
                throw new Exception("Failed loading patients");
            }
        }

        public override void BuildFilterModel()
        {
            var response = _api.Localities.Get();
            var localities = _api.Localities
                .ReadMany(response)
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
