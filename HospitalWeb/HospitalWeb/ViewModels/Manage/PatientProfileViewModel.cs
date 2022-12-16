using HospitalWeb.Filters.Models.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace HospitalWeb.ViewModels.Manage
{
    public class PatientProfileViewModel : ProfileViewModel
    {
        [Required(ErrorMessage = "AddressRequired")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "LocalityRequired")]
        [Display(Name = "Locality")]
        public string Locality { get; set; }
    }
}
