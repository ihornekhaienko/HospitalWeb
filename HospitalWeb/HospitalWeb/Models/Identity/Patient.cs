namespace HospitalWeb.Models.Identity
{
    public enum Sex
    {
        Male,
        Female
    }

    public class Patient : AppUser
    {
        public Patient()
        {
            Records = new List<Record>();
        }

        public Sex Sex { get; set; }
        public DateTime BirthDate { get; set; }

        public int AddressId { get; set; }
        public Address Address { get; set; }
        public ICollection<Record> Records { get; set; }
    }
}
