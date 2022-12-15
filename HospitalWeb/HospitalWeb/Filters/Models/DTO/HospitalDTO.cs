namespace HospitalWeb.Filters.Models.DTO
{
    public class HospitalDTO
    {
        public int HospitalId { get; set; }

        public string HospitalName { get; set; }

        public byte[] Image { get; set; }

        public string Address { get; set; }

        public int DoctorsCount { get; set; }
    }
}
