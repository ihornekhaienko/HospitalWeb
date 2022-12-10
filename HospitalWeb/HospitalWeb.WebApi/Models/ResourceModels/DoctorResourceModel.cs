using HospitalWeb.DAL.Entities;

namespace HospitalWeb.WebApi.Models.ResourceModels
{
    public class DoctorResourceModel : AppUserResourceModel
    {
        public int SpecialtyId { get; set; }

        public int HospitalId { get; set; }
    }
}
