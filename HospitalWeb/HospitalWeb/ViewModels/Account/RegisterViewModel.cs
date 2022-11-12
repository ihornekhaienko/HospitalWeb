using HospitalWeb.Attributes.Validation;
using System.ComponentModel.DataAnnotations;

namespace HospitalWeb.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "EmailRequired")]
        [EmailAddress(ErrorMessage = "EmailValidation")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "PhoneRequired")]
        [RegularExpression("^\\+?[1-9][0-9]{7,14}$")]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "PasswordRequired")]
        [StringLength(100, ErrorMessage = "PasswordValidation", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword")]
        [Compare("Password", ErrorMessage = "PasswordMatch")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "NameRequired")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "SurnameRequired")]
        [Display(Name = "Surname")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "BirthDateRequired")]
        [Display(Name = "BirthDate")]
        //[TimeLessThan("BirthDate")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "AddressRequired")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "LocalityRequired")]
        [Display(Name = "Locality")]
        public string Locality { get; set; }

        [Display(Name = "Sex")]
        public string Sex { get; set; }
    }
}
