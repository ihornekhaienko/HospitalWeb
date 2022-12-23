using HospitalWeb.Mvc.Filters.Builders.Interfaces;

namespace HospitalWeb.Mvc.Filters.Builders.Implementations
{
    public class ViewModelBuilderDirector
    {
        public void MakeViewModel(IViewModelBuilder builder)
        {
            builder.BuildEntityModel();
            builder.BuildFilterModel();
            builder.BuildSortModel();
            builder.BuildPageModel();
        }
    }
}
