using HospitalWeb.Domain.Entities.Identity;

namespace HospitalWeb.Domain.Entities
{
    public class Grade
    {
        public int GradeId { get; set; }

        public int Stars { get; set; }

        public string AuthorId { get; set; }
        public Patient Author { get; set; }

        public string TargetId { get; set; }
        public Doctor Target { get; set; }
    }
}
