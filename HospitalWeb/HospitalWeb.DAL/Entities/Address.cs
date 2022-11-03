using HospitalWeb.DAL.Entities.Identity;

namespace HospitalWeb.DAL.Entities
{
    public class Address
    {
        public int AddressId { get; set; }
        public string? FullAddress { get; set; }

        public ICollection<Patient>? Patients { get; set; }
        public Locality? Locality { get; set; }
    }
}
