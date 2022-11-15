﻿using HospitalWeb.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalWeb.Filters.Models.DTO
{
    public class AppointmentDTO
    {
        public int AppointmentId { get; set; }

        public string Diagnosis { get; set; }

        public string Prescription { get; set; }

        public DateTime AppointmentDate { get; set; }

        public string State { get; set; }
 
        public string DoctorId { get; set; }

        public string Doctor { get; set; }

        public string DoctorSpecialty { get; set; }

        public byte[] DoctorImage { get; set; }

        public string PatientId { get; set; }

        public string Patient { get; set; }

        public string PatientSex { get; set; }

        public string PatientBirthDate { get; set; }

        public byte[] PatientImage { get; set; }
    }
}