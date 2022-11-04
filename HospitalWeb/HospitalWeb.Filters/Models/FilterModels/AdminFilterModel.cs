namespace HospitalWeb.Filters.Models.FilterModels
{
    public class AdminFilterModel
    {
        public string? SearchString { get; private set; }

        public AdminFilterModel(string? searchString)
        {
            SearchString = searchString;
        }
    }
}
