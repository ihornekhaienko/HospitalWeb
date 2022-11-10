using HospitalWeb.Filters.Builders.Interfaces;

namespace HospitalWeb.Filters.Builders.Implementations
{
    public class ViewModelBuilderDirector
    {
        public void MakeViewModel(IViewModelBuilder builder)
        {
            builder.BuildFilterModel();
            builder.BuildSortModel();
            builder.BuildEntityModel();
            builder.BuildPageModel();
        }
    }
}
