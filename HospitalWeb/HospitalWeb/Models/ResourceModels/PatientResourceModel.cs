using HospitalWeb.Domain.Entities.Identity;

namespace HospitalWeb.Models.ResourceModels
{
    public class PatientResourceModel : AppUserResourceModel
    {
        public Sex Sex { get; set; }

        public DateTime BirthDate { get; set; }

        public int AddressId { get; set; }
    }
}
