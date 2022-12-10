using HospitalWeb.DAL.Entities.Identity;

namespace HospitalWeb.DAL.Entities
{
    public class Hospital
    {
        public int HospitalId { get; set; }

        public string HospitalName { get; set; }

        public byte[] Image { get; set; }

        public int AddressId { get; set; }

        public Address Address { get; set; }

        public ICollection<Doctor> Doctors { get; set; }
    }
}
