using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.Filters.Builders.Interfaces;
using HospitalWeb.Filters.Models;
using HospitalWeb.Filters.Models.DTO;
using HospitalWeb.Filters.Models.FilterModels;
using HospitalWeb.Filters.Models.SortModels;
using HospitalWeb.Filters.Models.ViewModels;
using HospitalWeb.Clients.Implementations;
using HospitalWeb.WebApi.Models.SortStates;

namespace HospitalWeb.Filters.Builders.Implementations
{
    public class DoctorsViewModelBuilder : ViewModelBuilder<DoctorsViewModel>
    {
        private readonly ApiUnitOfWork _api;
        private readonly int? _specialty;
        private readonly int? _hospital;
        private readonly int? _locality;
        private readonly DoctorSortState _sortOrder;
        private IEnumerable<DoctorDTO> _doctors;
        private PageModel _pageModel;
        private DoctorFilterModel _filterModel;
        private DoctorSortModel _sortModel;
        private int _count = 0;

        public DoctorsViewModelBuilder(
            ApiUnitOfWork api, 
            int pageNumber, 
            string searchString,
            DoctorSortState sortOrder, 
            int? specialty = null,
            int? hospital = null,
            int? locality = null,
            int pageSize = 10
            ) : base(pageNumber, pageSize, searchString)
        {
            _api = api;
            _sortOrder = sortOrder;
            _specialty = specialty;
            _hospital = hospital;
            _locality = locality;
        }

        public override void BuildEntityModel()
        {
            var response = _api.Doctors.Filter(_searchString, _specialty, _hospital, _locality, _sortOrder, _pageSize, _pageNumber);

            if (response.IsSuccessStatusCode)
            {
                _doctors = _api.Doctors.ReadMany(response)
                    .Select(d => new DoctorDTO
                    {
                        Id = d.Id,
                        Name = d.Name,
                        Surname = d.Surname,
                        Email = d.Email,
                        PhoneNumber = d.PhoneNumber,
                        Image = d.Image,
                        Specialty = d.Specialty.SpecialtyName,
                        Hospital = d.Hospital.HospitalName,
                        Locality = d.Hospital.Address.Locality.LocalityName,
                        Rating = d.Rating
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
            var response = _api.Specialties.Get();

            var specialties = _api.Specialties
                .ReadMany(response)
                .OrderBy(s => s.SpecialtyName)
                .Select(s => new SpecialtyDTO { SpecialtyId = s.SpecialtyId, SpecialtyName = s.SpecialtyName })
                .ToList();

            response = _api.Hospitals.Get();

            var hospitals = _api.Hospitals
               .ReadMany(response)
               .OrderBy(h => h.HospitalName)
               .Select(h => new HospitalDTO { HospitalId = h.HospitalId, HospitalName = h.HospitalName })
               .ToList();

            response = _api.Localities.Get();

            var localities = _api.Localities
               .ReadMany(response)
               .OrderBy(l => l.LocalityName)
               .Select(l => new LocalityDTO { LocalityId = l.LocalityId, LocalityName = l.LocalityName })
               .ToList();

            _filterModel = new DoctorFilterModel(_searchString, specialties, _specialty, hospitals, _hospital, localities, _locality);
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
