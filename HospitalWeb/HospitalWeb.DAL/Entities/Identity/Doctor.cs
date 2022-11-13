﻿using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalWeb.DAL.Entities.Identity
{
    [Table("Doctors")]
    public class Doctor : AppUser
    {
        public Doctor()
        {
            Appointments = new List<Appointment>();
            Schedules = new List<Schedule>();
        }

        public Specialty Specialty { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<Schedule> Schedules { get; set; }
    }
}
