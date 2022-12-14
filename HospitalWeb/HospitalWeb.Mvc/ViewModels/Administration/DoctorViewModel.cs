using System.ComponentModel.DataAnnotations;

namespace HospitalWeb.Mvc.ViewModels.Administration
{
    public class DoctorViewModel
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

        [Required(ErrorMessage = "SpecialtyRequired")]
        [Display(Name = "Specialty")]
        public string Specialty { get; set; }

        [Required(ErrorMessage = "HospitalRequired")]
        [Display(Name = "Hospital")]
        public int Hospital { get; set; }

        [Display(Name = "Price")]
        public decimal ServicePrice { get; set; }
    }
}
