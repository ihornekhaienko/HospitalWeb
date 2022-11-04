namespace HospitalWeb.Filters.Models.SortModels
{
    public enum AdminSortState
    {
        IdAsc,
        IdDesc,
        NameAsc,
        NameDesc,
        SurnameAsc,
        SurnameDesc,
        EmailAsc,
        EmailDesc
    }

    public class AdminSortModel
    {
        public AdminSortState IdSort { get; private set; }
        public AdminSortState NameSort { get; private set; }
        public AdminSortState SurnameSort { get; private set; }
        public AdminSortState EmailSort { get; private set; }

        public AdminSortState Current { get; private set; }

        public AdminSortModel(AdminSortState sortOrder)
        {
            IdSort = sortOrder == AdminSortState.IdAsc ? AdminSortState.IdDesc : AdminSortState.IdAsc;
            NameSort = sortOrder == AdminSortState.NameAsc ? AdminSortState.NameDesc : AdminSortState.NameAsc;
            SurnameSort = sortOrder == AdminSortState.SurnameAsc ? AdminSortState.SurnameDesc : AdminSortState.SurnameAsc;
            EmailSort = sortOrder == AdminSortState.EmailAsc ? AdminSortState.EmailDesc : AdminSortState.EmailAsc;
            Current = sortOrder;
        }
    }
}
