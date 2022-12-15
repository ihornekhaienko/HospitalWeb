namespace HospitalWeb.Filters.Builders.Interfaces
{
    public interface IViewModelBuilder
    {
        public void BuildPageModel();
        public void BuildFilterModel();
        public void BuildSortModel();
        public void BuildEntityModel();
    }
}
