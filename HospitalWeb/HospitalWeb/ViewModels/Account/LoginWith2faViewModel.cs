using System.ComponentModel.DataAnnotations;

namespace HospitalWeb.ViewModels.Account
{
    public class LoginWith2faViewModel
    {
        [Required]
        [StringLength(7, ErrorMessage = "VerificationCodeErrorMessage", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "AuthenticatorCode")]
        public string TwoFactorCode { get; set; }

        public bool RememberMe { get; set; }

        [Display(Name = "RememberDevice")]
        public bool RememberDevice { get; set; }
    }
}
