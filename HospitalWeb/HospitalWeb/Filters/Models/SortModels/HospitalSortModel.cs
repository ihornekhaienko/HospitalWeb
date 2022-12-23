using HospitalWeb.Models.SortStates;

namespace HospitalWeb.Filters.Models.SortModels
{
    public class HospitalSortModel
    {
        public HospitalSortState IdSort { get; private set; }
        public HospitalSortState NameSort { get; private set; }
        public HospitalSortState AddressSort { get; private set; }
        public HospitalSortState DoctorsCountSort { get; private set; }

        public HospitalSortState Current { get; private set; }

        public HospitalSortModel(HospitalSortState sortOrder)
        {
            IdSort = HospitalSortState.Id;
            NameSort = sortOrder == HospitalSortState.NameAsc ? HospitalSortState.NameDesc : HospitalSortState.NameAsc;
            AddressSort = sortOrder == HospitalSortState.AddressAsc ? HospitalSortState.AddressDesc : HospitalSortState.AddressAsc;
            DoctorsCountSort = sortOrder == HospitalSortState.DoctorsCountAsc ? HospitalSortState.DoctorsCountDesc : HospitalSortState.DoctorsCountAsc;

            Current = sortOrder;
        }
    }
}
