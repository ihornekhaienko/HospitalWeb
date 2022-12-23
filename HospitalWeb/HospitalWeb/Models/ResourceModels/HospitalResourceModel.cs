using HospitalWeb.Domain.Entities;

namespace HospitalWeb.Models.ResourceModels
{
    public class HospitalResourceModel
    {
        public int HospitalId { get; set; }

        public string HospitalName { get; set; }

        public byte[] Image { get; set; }

        public int AddressId { get; set; }

        public HospitalType Type { get; set; }
    }
}
