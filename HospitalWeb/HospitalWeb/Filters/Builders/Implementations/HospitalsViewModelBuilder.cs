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
    public class HospitalsViewModelBuilder : ViewModelBuilder<HospitalsViewModel>
    {
        private readonly ApiUnitOfWork _api;
        private readonly int? _locality;
        private readonly HospitalSortState _sortOrder;
        private IEnumerable<HospitalDTO> _hospitals;
        private PageModel _pageModel;
        private HospitalFilterModel _filterModel;
        private HospitalSortModel _sortModel;
        private int _count = 0;

        public HospitalsViewModelBuilder(
           ApiUnitOfWork api,
           int pageNumber,
           string searchString,
           HospitalSortState sortOrder,
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
            var response = _api.Hospitals.Filter(_searchString, _locality, _sortOrder, _pageSize, _pageNumber);

            if (response.IsSuccessStatusCode)
            {
                _hospitals = _api.Hospitals.ReadMany(response)
                    .Select(h => new HospitalDTO
                    {
                        HospitalId = h.HospitalId,
                        HospitalName = h.HospitalName,
                        DoctorsCount = h.Doctors.Count,
                        Image = h.Image,
                        Address = h.Address.ToString()
                    });

                _count = Convert.ToInt32(response.Headers.GetValues("TotalCount").FirstOrDefault());
            }
            else
            {
                throw new Exception("Failed loading hospitals");
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

            _filterModel = new HospitalFilterModel(_searchString, localities, _locality);
        }

        public override void BuildPageModel()
        {
            _pageModel = new PageModel(_count, _pageNumber, _pageSize);
        }

        public override void BuildSortModel()
        {
            _sortModel = new HospitalSortModel(_sortOrder);
        }

        public override HospitalsViewModel GetViewModel()
        {
            if (_pageModel == null || _sortModel == null || _filterModel == null)
            {
                throw new Exception("Failed building view model, some of models is null");
            }

            return new HospitalsViewModel
            {
                Hospitals = _hospitals,
                PageModel = _pageModel,
                SortModel = _sortModel,
                FilterModel = _filterModel
            };
        }
    }
}
