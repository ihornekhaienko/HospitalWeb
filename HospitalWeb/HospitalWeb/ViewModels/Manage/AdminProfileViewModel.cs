using System.ComponentModel.DataAnnotations;

namespace HospitalWeb.ViewModels.Manage
{
    public class AdminProfileViewModel
    {
        [Required(ErrorMessage = "NameRequired")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "SurnameRequired")]
        [Display(Name = "Surname")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "EmailRequired")]
        [EmailAddress(ErrorMessage = "EmailValidation")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "PhoneRequired")]
        [Display(Name = "Phone")]
        [RegularExpression("^\\+?[1-9][0-9]{7,14}$")]
        public string Phone { get; set; }

        public byte[] Image { get; set; }

        public bool IsSuperAdmin { get; set; }
    }
}
