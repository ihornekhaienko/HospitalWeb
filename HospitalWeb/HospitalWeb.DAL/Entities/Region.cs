namespace HospitalWeb.DAL.Entities
{
    public class Region
    {
        public Region()
        {
            Localities = new List<Locality>();
        }

        public int RegionId { get; set; }
        public string RegionName { get; set; }

        public ICollection<Locality> Localities { get; set; }
    }
}
