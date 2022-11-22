using Bogus;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Entities.Identity;

namespace HospitalWeb.WebApi.Tests
{
    internal static class DataGenerator
    {
        internal static List<Address> GetTestAddresses()
        {
            int id = 1;

            var faker = new Faker<Address>()
                .RuleFor(a => a.AddressId, f => id++)
                .RuleFor(a => a.FullAddress, f => f.Address.StreetAddress());

            var testAddresses = faker.Generate(100);

            return testAddresses;
        }

        internal static List<Address> GetTestAddresses(IEnumerable<Locality> localities)
        {
            int id = 1;

            var faker = new Faker<Address>()
                .RuleFor(a => a.AddressId, f => id++)
                .RuleFor(a => a.FullAddress, f => f.Address.StreetAddress())
                .RuleFor(a => a.Locality, f => f.PickRandom(localities));

            var testAddresses = faker.Generate(100);

            return testAddresses;
        }

        internal static List<Appointment> GetTestAppointments()
        {
            int id = 1;
            var states = new[] { State.Completed, State.Missed, State.Planned, State.Canceled };

            var faker = new Faker<Appointment>()
                .RuleFor(a => a.AppointmentId, f => id++)
                .RuleFor(a => a.State, f => f.PickRandom(states[1..2]))
                .RuleFor(a => a.Prescription, f => f.Lorem.Text());

            var testAppointments = faker.Generate(50);

            return testAppointments;
        }

        internal static List<Appointment> GetTestAppointments(IEnumerable<Diagnosis> diagnoses, IEnumerable<Doctor> doctors, IEnumerable<Patient> patients)
        {
            int id = 1;
            var states = new[] { State.Completed, State.Missed, State.Planned, State.Canceled };

            var faker = new Faker<Appointment>()
                .RuleFor(a => a.AppointmentId, f => id++)
                .RuleFor(a => a.State, f => f.PickRandom(states[1..2]))
                .RuleFor(a => a.Prescription, f => f.Lorem.Text())
                .RuleFor(a => a.Diagnosis, f => f.PickRandom(diagnoses))
                .RuleFor(a => a.Doctor, f => f.PickRandom(doctors))
                .RuleFor(a => a.Patient, f => f.PickRandom(patients));

            var testAppointments = faker.Generate(50);

            return testAppointments;
        }

        internal static List<Diagnosis> GetTestDiagnoses()
        {
            var diagnoses = new[] { "Survey", "SARS", "Gastritis", "Ulcer", "Pancreatitis", "Twist" };

            int id = 1;

            var faker = new Faker<Diagnosis>()
                .RuleFor(d => d.DiagnosisName, f => diagnoses[id - 1])
                .RuleFor(d => d.DiagnosisId, f => id++);

            var testDiagnoses = faker.Generate(diagnoses.Length);

            return testDiagnoses;
        }

        internal static List<Locality> GetTestLocalities()
        {
            int id = 1;

            var faker = new Faker<Locality>()
                .RuleFor(l => l.LocalityId, f => id++)
                .RuleFor(l => l.LocalityName, f => f.Address.City());

            var testLocalities = faker.Generate(100);

            return testLocalities;
        }

        internal static List<Schedule> GetTestSchedules()
        {
            int id = 1;

            var faker = new Faker<Schedule>()
                .RuleFor(s => s.ScheduleId, f => id++)
                .RuleFor(s => s.StartTime, f => f.Date.Between(new DateTime(2000, 1, 1, 8, 0, 0), new DateTime(2000, 1, 1, 13, 0, 0)))
                .RuleFor(s => s.EndTime, f => f.Date.Between(new DateTime(2000, 1, 1, 13, 0, 0), new DateTime(2000, 1, 1, 18, 0, 0)))
                .RuleFor(s => s.DayOfWeek, f =>
                {
                    DayOfWeek day;
                    Enum.TryParse(f.Date.Weekday(), out day);

                    return day;
                });

            var testSchedules = faker.Generate(100);

            return testSchedules;
        }

        internal static List<Schedule> GetTestSchedules(IEnumerable<Doctor> doctors)
        {
            int id = 1;

            var faker = new Faker<Schedule>()
                .RuleFor(s => s.ScheduleId, f => id++)
                .RuleFor(s => s.StartTime, f => f.Date.Between(new DateTime(2000, 1, 1, 8, 0, 0), new DateTime(2000, 1, 1, 13, 0, 0)))
                .RuleFor(s => s.EndTime, f => f.Date.Between(new DateTime(2000, 1, 1, 13, 0, 0), new DateTime(2000, 1, 1, 18, 0, 0)))
                .RuleFor(s => s.DayOfWeek, f =>
                {
                    DayOfWeek day;
                    Enum.TryParse(f.Date.Weekday(), out day);

                    return day;
                })
                .RuleFor(s => s.Doctor, f => f.PickRandom(doctors));

            var testSchedules = faker.Generate(100);

            return testSchedules;
        }

        internal static List<Specialty> GetTestSpecialties()
        {
            var specialties = new[] { "Therapist", "Surgeon", "Gastroenterologist", "Dentist", "Neurologist", "Dermatologist" };

            int id = 1;

            var faker = new Faker<Specialty>()
                .RuleFor(s => s.SpecialtyName, f => specialties[id - 1])
                .RuleFor(s => s.SpecialtyId, f => id++);

            var testSpecialties = faker.Generate(specialties.Length);

            return testSpecialties;
        }

        internal static List<AppUser> GetTestAppUsers()
        {
            var levels = new[] { true, false };

            var faker = new Faker<AppUser>()
                .RuleFor(u => u.Id, f => f.UniqueIndex.ToString())
                .RuleFor(u => u.UserName, f => f.Internet.Email())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.Name, f => f.Name.FirstName())
                .RuleFor(u => u.Surname, f => f.Name.LastName());

            var testUsers = faker.Generate(100);

            return testUsers;
        }

        internal static List<Admin> GetTestAdmins()
        {
            var levels = new[] { true, false };

            var faker = new Faker<Admin>()
                .RuleFor(u => u.Id, f => f.UniqueIndex.ToString())
                .RuleFor(u => u.UserName, f => f.Internet.Email())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.Name, f => f.Name.FirstName())
                .RuleFor(u => u.Surname, f => f.Name.LastName())
                .RuleFor(a => a.IsSuperAdmin, f => f.PickRandom(levels));

            var testAdmins = faker.Generate(100);

            return testAdmins;
        }

        internal static List<Doctor> GetTestDoctors()
        {
            var faker = new Faker<Doctor>()
                .RuleFor(u => u.Id, f => f.UniqueIndex.ToString())
                .RuleFor(u => u.UserName, f => f.Internet.Email())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.Name, f => f.Name.FirstName())
                .RuleFor(u => u.Surname, f => f.Name.LastName());

            var testDoctors = faker.Generate(100);

            return testDoctors;
        }

        internal static List<Doctor> GetTestDoctors(IEnumerable<Specialty> specialties)
        {
            var faker = new Faker<Doctor>()
                .RuleFor(u => u.Id, f => f.UniqueIndex.ToString())
                .RuleFor(u => u.UserName, f => f.Internet.Email())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.Name, f => f.Name.FirstName())
                .RuleFor(u => u.Surname, f => f.Name.LastName())
                .RuleFor(d => d.Specialty, f => f.PickRandom(specialties));

            var testDoctors = faker.Generate(100);

            return testDoctors;
        }

        internal static List<Patient> GetTestPatients()
        {
            var sexes = new[] { Sex.Male, Sex.Female };

            var faker = new Faker<Patient>()
                .RuleFor(u => u.Id, f => f.UniqueIndex.ToString())
                .RuleFor(u => u.UserName, f => f.Internet.Email())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.Name, f => f.Name.FirstName())
                .RuleFor(u => u.Surname, f => f.Name.LastName())
                .RuleFor(p => p.Sex, f => f.PickRandom(sexes))
                .RuleFor(p => p.BirthDate, f => f.Date.Past(70, new DateTime(2000, 1, 1)));

            var testPatients = faker.Generate(100);

            return testPatients;
        }

        internal static List<Patient> GetTestPatients(IEnumerable<Address> addresses)
        {
            var sexes = new[] { Sex.Male, Sex.Female };

            var faker = new Faker<Patient>()
                .RuleFor(u => u.Id, f => f.UniqueIndex.ToString())
                .RuleFor(u => u.UserName, f => f.Internet.Email())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.Name, f => f.Name.FirstName())
                .RuleFor(u => u.Surname, f => f.Name.LastName())
                .RuleFor(p => p.Sex, f => f.PickRandom(sexes))
                .RuleFor(p => p.BirthDate, f => f.Date.Past(70, new DateTime(2000, 1, 1)))
                .RuleFor(p => p.Address, f => f.PickRandom(addresses));

            var testPatients = faker.Generate(100);

            return testPatients;
        }
    }
}
