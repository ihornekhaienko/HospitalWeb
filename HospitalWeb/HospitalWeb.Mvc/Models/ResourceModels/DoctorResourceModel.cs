using HospitalWeb.Domain.Entities;

namespace HospitalWeb.Mvc.Models.ResourceModels
{
    public class DoctorResourceModel : AppUserResourceModel
    {
        public int SpecialtyId { get; set; }

        public int HospitalId { get; set; }

        public decimal ServicePrice { get; set; }
    }
}
