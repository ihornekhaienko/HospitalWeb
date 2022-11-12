using System.ComponentModel.DataAnnotations;

namespace HospitalWeb.ViewModels.Administration
{
    public class PatientViewModel
    {
        [Required(ErrorMessage = "EmailRequired")]
        [EmailAddress(ErrorMessage = "EmailValidation")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "NameRequired")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "SurnameRequired")]
        [Display(Name = "Surname")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "PhoneRequired")]
        [RegularExpression("^\\+?[1-9][0-9]{7,14}$")]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "AddressRequired")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "LocalityRequired")]
        [Display(Name = "Locality")]
        public string Locality { get; set; }
    }
}
