namespace HospitalWeb.WebApi.Models.ResourceModels
{
    public class AdminResourceModel
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public string PhoneNumber { get; set; }

        public string Password { get; set; }

        public bool IsSuperAdmin { get; set; }

        public bool EmailConfirmed { get; set; }
    }
}
