﻿using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalWeb.DAL.Entities.Identity
{
    public enum Sex
    {
        Male,
        Female
    }

    [Table("Patients")]
    public class Patient : AppUser
    {
        public Patient()
        {
            Appointments = new List<Appointment>();
        }

        public Sex Sex { get; set; }
        public DateTime BirthDate { get; set; }

        public Address Address { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
    }
}
