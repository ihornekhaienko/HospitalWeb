using System.ComponentModel.DataAnnotations;

namespace HospitalWeb.ViewModels.Account
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "EmailRequired")]
        [EmailAddress(ErrorMessage = "EmailValidation")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "PasswordRequired")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword")]
        [Compare("Password", ErrorMessage = "PasswordMatch")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
