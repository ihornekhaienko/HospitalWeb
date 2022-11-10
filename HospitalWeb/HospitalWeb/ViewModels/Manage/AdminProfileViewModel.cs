using System.ComponentModel.DataAnnotations;

namespace HospitalWeb.ViewModels.Manage
{
    public class AdminProfileViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Surname")]
        public string Surname { get; set; }

        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Phone number")]
        [RegularExpression("^\\+?[1-9][0-9]{7,14}$")]
        public string Phone { get; set; }

        public byte[] Image { get; set; }

        public bool IsSuperAdmin { get; set; }
    }
}
