using System.ComponentModel.DataAnnotations;

namespace HospitalWeb.Mvc.ViewModels.Account
{
    public class LoginWithRecoveryCodeViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "RecoveryCode")]
        public string RecoveryCode { get; set; }
    }
}
