namespace HospitalWeb.WebApi.Models.ResourceModels
{
    public class AppUserResourceModel
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public string PhoneNumber { get; set; }

        public string Password { get; set; }

        public string NewPassword { get; set; }

        public bool EmailConfirmed { get; set; }

        public byte[] Image { get; set; }

        public string CalendarId { get; set; }
    }
}
