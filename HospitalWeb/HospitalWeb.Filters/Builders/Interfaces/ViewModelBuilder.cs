namespace HospitalWeb.Filters.Builders.Interfaces
{
    public abstract class ViewModelBuilder<T> : IViewModelBuilder
    {
        protected readonly int _pageNumber;
        protected readonly int _pageSize;
        protected readonly string _searchString;

        public ViewModelBuilder(int pageNumber, int pageSize, string searchString)
        {
            _pageNumber = pageNumber;
            _pageSize = pageSize;
            _searchString = searchString;
        }

        public abstract void BuildPageModel();
        public abstract void BuildFilterModel();
        public abstract void BuildSortModel();
        public abstract void BuildEntityModel();
        public abstract T GetViewModel();
    }
}
