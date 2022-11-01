namespace HospitalWeb.Models
{
    public class Locality
    {
        public int LocalityId { get; set; }
        public string LocalityName { get; set; }

        public int RegionId { get; set; }
        public Region Region { get; set; }
    }
}
