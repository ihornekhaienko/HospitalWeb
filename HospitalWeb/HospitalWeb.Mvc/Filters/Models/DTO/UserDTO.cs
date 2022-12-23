namespace HospitalWeb.Mvc.Filters.Models.DTO
{
    public class UserDTO
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public byte[] Image { get; set; }

        public override string ToString()
        {
            return $"{Name} {Surname}";
        }
    }
}
