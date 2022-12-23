using HospitalWeb.Mvc.Filters.Models.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace HospitalWeb.Mvc.ViewModels.Manage
{
    public class DoctorProfileViewModel : ProfileViewModel
    {
        public string Specialty { get; set; }
    }
}
