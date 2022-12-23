using HospitalWeb.Mvc.Filters.Models.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace HospitalWeb.Mvc.ViewModels.Manage
{
    public class AdminProfileViewModel : ProfileViewModel
    {
        public bool IsSuperAdmin { get; set; }
    }
}
