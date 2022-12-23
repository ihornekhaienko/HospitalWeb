using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace HospitalWeb.Mvc.ViewModels.Manage
{
    public class Enable2faViewModel
    {
        [Required]
        [StringLength(7, ErrorMessage = "AuthenticationCodeErrorMessage", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Verification Code")]
        public string Code { get; set; }

        [BindNever]
        public string SharedKey { get; set; }

        [BindNever]
        public string AuthenticatorUri { get; set; }
    }
}
