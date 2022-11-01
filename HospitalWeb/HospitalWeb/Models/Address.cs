using HospitalWeb.Models.Identity;

namespace HospitalWeb.Models
{
    public class Address
    {
        public int AddressId { get; set; }
        public string Street { get; set; }

        public int LocalityId { get; set; }
        public Locality Locality { get; set; }
        public ICollection<Patient> Patients { get; set; }
    }
}
