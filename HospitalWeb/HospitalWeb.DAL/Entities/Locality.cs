namespace HospitalWeb.DAL.Entities
{
    public class Locality
    {
        public int LocalityId { get; set; }

        public string LocalityName { get; set; }

        public ICollection<Address> Addresses { get; set; }
    }
}
