namespace HospitalWeb.Mvc.ViewModels.Doctors
{
    public class DoctorDetailsViewModel
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public byte[] Image { get; set; }

        public string Specialty { get; set; }

        public string Hospital { get; set; }

        public decimal ServicePrice { get; set; }

        public double Rating { get; set; }

        public int? MyGrade { get; set; }
    }
}
