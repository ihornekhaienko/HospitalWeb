using HospitalWeb.Filters.Models.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace HospitalWeb.ViewModels.Manage
{
    public class AdminProfileViewModel : ProfileViewModel
    {
        public bool IsSuperAdmin { get; set; }
    }
}
