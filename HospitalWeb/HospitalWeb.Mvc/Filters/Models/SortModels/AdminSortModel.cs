using HospitalWeb.Mvc.Models.SortStates;

namespace HospitalWeb.Mvc.Filters.Models.SortModels
{
    public class AdminSortModel
    {
        public AdminSortState IdSort { get; private set; }
        public AdminSortState NameSort { get; private set; }
        public AdminSortState SurnameSort { get; private set; }
        public AdminSortState EmailSort { get; private set; }
        public AdminSortState PhoneSort { get; protected set; }
        public AdminSortState LevelSort { get; private set; }

        public AdminSortState Current { get; private set; }

        public AdminSortModel(AdminSortState sortOrder)
        {
            IdSort = AdminSortState.Id;
            NameSort = sortOrder == AdminSortState.NameAsc ? AdminSortState.NameDesc : AdminSortState.NameAsc;
            SurnameSort = sortOrder == AdminSortState.SurnameAsc ? AdminSortState.SurnameDesc : AdminSortState.SurnameAsc;
            EmailSort = sortOrder == AdminSortState.EmailAsc ? AdminSortState.EmailDesc : AdminSortState.EmailAsc;
            PhoneSort = sortOrder == AdminSortState.PhoneAsc ? AdminSortState.PhoneDesc : AdminSortState.PhoneAsc;
            LevelSort = sortOrder == AdminSortState.LevelAsc ? AdminSortState.LevelDesc : AdminSortState.LevelAsc;
            Current = sortOrder;
        }
    }
}
