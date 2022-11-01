namespace HospitalWeb.Models.Identity
{
    public class Doctor : User
    {
        public Doctor()
        {
            Records = new List<Record>();
            Schedules = new List<Schedule>();
        }

        public int SpecialtyId { get; set; }
        public Specialty Specialty { get; set; }
        public ICollection<Record> Records { get; set; }
        public ICollection<Schedule> Schedules { get; set; }
    }
}
