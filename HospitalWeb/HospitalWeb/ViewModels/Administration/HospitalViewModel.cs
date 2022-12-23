using HospitalWeb.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace HospitalWeb.ViewModels.Administration
{
    public class HospitalViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "TitleRequired")]
        [Display(Name = "Name")]
        public string Name { get; set; } 

        [Required(ErrorMessage = "TypeRequired")]
        [Display(Name = "Type")]
        public HospitalType Type { get; set; }

        [Required(ErrorMessage = "AddressRequired")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "LocalityRequired")]
        [Display(Name = "Locality")]
        public string Locality { get; set; }

        public byte[] Image { get; set; }
    }
}
