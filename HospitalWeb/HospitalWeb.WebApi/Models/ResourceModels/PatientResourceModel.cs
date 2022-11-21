using HospitalWeb.DAL.Entities.Identity;

namespace HospitalWeb.WebApi.Models.ResourceModels
{
    public class PatientResourceModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public string PhoneNumber { get; set; }

        public string Password { get; set; }

        public Sex Sex { get; set; }

        public DateTime BirthDate { get; set; }

        public int AddressId { get; set; }
    }
}
