using HospitalWeb.Filters.Models.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace HospitalWeb.ViewModels.Manage
{
    public class DoctorProfileViewModel : ProfileViewModel
    {
        public string Specialty { get; set; }
    }
}
