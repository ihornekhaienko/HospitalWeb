﻿using HospitalWeb.DAL.Entities.Identity;

namespace HospitalWeb.DAL.Entities
{
    public class Hospital
    {
        public int HospitalId { get; set; }

        public string HospitalName { get; set; }

        public byte[] Image { get; set; }

        public int AddressId { get; set; }

        public Address Address { get; set; }

        public ICollection<Doctor> Doctors { get; set; }

        public double Rating 
        {
            get
            {
                var rated = Doctors?.Where(d => d.Rating != 0).ToList();

                if (rated == null || rated.Count == 0)
                {
                    return 0;
                }

                return rated.Average(d => d.Rating);
            }
        }
    }
}
