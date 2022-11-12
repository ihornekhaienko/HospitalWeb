using System.ComponentModel.DataAnnotations;

namespace HospitalWeb.ViewModels.Manage
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "PasswordRequired")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "NewPasswordRequired")]
        [DataType(DataType.Password)]
        [Display(Name = "NewPassword")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword")]
        [Compare("NewPassword", ErrorMessage = "PasswordMatch")]
        public string ConfirmPassword { get; set; }
    }
}
