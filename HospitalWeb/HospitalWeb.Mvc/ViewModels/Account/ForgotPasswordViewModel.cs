using System.ComponentModel.DataAnnotations;

namespace HospitalWeb.Mvc.ViewModels.Account
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "EmailRequired")]
        [EmailAddress(ErrorMessage = "EmailValidation")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
