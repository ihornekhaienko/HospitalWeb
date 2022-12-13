using Bogus;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Entities.Identity;

namespace HospitalWeb.Services.Extensions
{
    public static class DataGenerator
    {
        public static List<Address> GetTestAddresses(int count = 100)
        {
            int id = 1;

            var faker = new Faker<Address>()
                .RuleFor(a => a.AddressId, f => id++)
                .RuleFor(a => a.FullAddress, f => f.Address.StreetAddress());

            var testAddresses = faker.Generate(count);

            return testAddresses;
        }

        public static List<Address> GetTestAddresses(IEnumerable<Locality> localities, int count = 100)
        {
            var faker = new Faker<Address>()
                .RuleFor(a => a.FullAddress, f => f.Address.StreetAddress())
                .RuleFor(a => a.Locality, f => f.PickRandom(localities));

            var testAddresses = faker.Generate(count);

            return testAddresses;
        }

        public static List<Address> GetTestAddressesWithId(IEnumerable<Locality> localities, int count = 100)
        {
            int id = 1;

            var faker = new Faker<Address>()
                .RuleFor(a => a.AddressId, f => id++)
                .RuleFor(a => a.FullAddress, f => f.Address.StreetAddress())
                .RuleFor(a => a.Locality, f => f.PickRandom(localities));

            var testAddresses = faker.Generate(count);

            return testAddresses;
        }

        public static List<Appointment> GetTestAppointments(int count = 100)
        {
            int id = 1;
            var states = new[] { State.Completed, State.Missed, State.Planned, State.Canceled };

            var faker = new Faker<Appointment>()
                .RuleFor(a => a.AppointmentId, f => id++)
                .RuleFor(a => a.State, f => f.PickRandom(states[1..2]))
                .RuleFor(a => a.Prescription, f => f.Lorem.Text())
                .RuleFor(a => a.AppointmentDate, f => f.Date.Past(refDate: DateTime.Today));

            var testAppointments = faker.Generate(count);

            return testAppointments;
        }

        public static List<Appointment> GetTestAppointments(
            IEnumerable<Diagnosis> diagnoses,
            IEnumerable<Doctor> doctors,
            IEnumerable<Patient> patients,
            int count = 100)
        {
            var states = new[] { State.Completed, State.Missed, State.Planned, State.Canceled };

            var faker = new Faker<Appointment>()
                .RuleFor(a => a.State, f => f.PickRandom(states[1..2]))
                .RuleFor(a => a.Prescription, f => f.Lorem.Text())
                .RuleFor(a => a.AppointmentDate, f => f.Date.Past(refDate: DateTime.Today))
                .RuleFor(a => a.Diagnosis, f => f.PickRandom(diagnoses))
                .RuleFor(a => a.Doctor, f => f.PickRandom(doctors))
                .RuleFor(a => a.Patient, f => f.PickRandom(patients));

            var testAppointments = faker.Generate(count);

            return testAppointments;
        }

        public static List<Appointment> GetTestAppointmentsWithId(
            IEnumerable<Diagnosis> diagnoses,
            IEnumerable<Doctor> doctors,
            IEnumerable<Patient> patients,
            int count = 100)
        {
            int id = 1;
            var states = new[] { State.Completed, State.Missed, State.Planned, State.Canceled };

            var faker = new Faker<Appointment>()
                .RuleFor(a => a.AppointmentId, f => id++)
                .RuleFor(a => a.State, f => f.PickRandom(states[1..2]))
                .RuleFor(a => a.Prescription, f => f.Lorem.Text())
                .RuleFor(a => a.AppointmentDate, f => f.Date.Past(refDate: DateTime.Today))
                .RuleFor(a => a.Diagnosis, f => f.PickRandom(diagnoses))
                .RuleFor(a => a.Doctor, f => f.PickRandom(doctors))
                .RuleFor(a => a.Patient, f => f.PickRandom(patients));

            var testAppointments = faker.Generate(count);

            return testAppointments;
        }

        public static List<Diagnosis> GetTestDiagnoses(int count = 100)
        {
            var faker = new Faker<Diagnosis>()
                .RuleFor(d => d.DiagnosisName, f => f.Lorem.Word());

            var testDiagnoses = faker.Generate(count);

            return testDiagnoses;
        }

        public static List<Diagnosis> GetTestDiagnosesWithId(int count = 100)
        {
            int id = 1;

            var faker = new Faker<Diagnosis>()
                .RuleFor(d => d.DiagnosisId, f => id++)
                .RuleFor(d => d.DiagnosisName, f => f.Lorem.Word());

            var testDiagnoses = faker.Generate(count);

            return testDiagnoses;
        }

        public static List<Hospital> GetTestHospitals(int count = 100)
        {
            int id = 1;

            var faker = new Faker<Hospital>()
                .RuleFor(h => h.HospitalId, f => id)
                .RuleFor(h => h.HospitalName, f => "Hospital " + id++);

            var testHospitals = faker.Generate(count);

            return testHospitals;
        }

        public static List<Hospital> GetTestHospitals(
            IEnumerable<Address> addresses,
            int count = 100)
        {
            int id = 1;

            var faker = new Faker<Hospital>()
                .RuleFor(h => h.HospitalName, f => "Hospital " + id++)
                .RuleFor(h => h.Address, f => f.PickRandom(addresses));

            var testHospitals = faker.Generate(count);

            return testHospitals;
        }

        public static List<Hospital> GetTestHospitalsWithId(
           IEnumerable<Address> addresses,
           int count = 100)
        {
            int id = 1;

            var faker = new Faker<Hospital>()
                .RuleFor(h => h.HospitalId, f => id)
                .RuleFor(h => h.HospitalName, f => "Hospital " + id++)
                .RuleFor(h => h.Address, f => f.PickRandom(addresses));

            var testHospitals = faker.Generate(count);

            return testHospitals;
        }

        public static List<Locality> GetTestLocalities(int count = 100)
        {
            var faker = new Faker<Locality>()
                .RuleFor(l => l.LocalityName, f => f.Address.City());

            var testLocalities = faker.Generate(count);

            return testLocalities;
        }

        public static List<Locality> GetTestLocalitiesWithId(int count = 100)
        {
            int id = 1;

            var faker = new Faker<Locality>()
                .RuleFor(l => l.LocalityId, f => id++)
                .RuleFor(l => l.LocalityName, f => f.Address.City());

            var testLocalities = faker.Generate(count);

            return testLocalities;
        }

        public static List<Schedule> GetTestSchedules(int count = 100)
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

            var testSchedules = faker.Generate(count);

            return testSchedules;
        }

        public static List<Schedule> GetTestSchedules(IEnumerable<Doctor> doctors, int count = 100)
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

            var testSchedules = faker.Generate(count);

            return testSchedules;
        }

        public static List<Specialty> GetTestSpecialties()
        {
            var specialties = new[] { "Therapist", "Surgeon", "Gastroenterologist", "Dentist", "Neurologist", "Dermatologist" };

            int id = 1;

            var faker = new Faker<Specialty>()
                .RuleFor(s => s.SpecialtyName, f => specialties[id++ - 1]);

            var testSpecialties = faker.Generate(specialties.Length);

            return testSpecialties;
        }

        public static List<Specialty> GetTestSpecialtiesWithId()
        {
            var specialties = new[] { "Therapist", "Surgeon", "Gastroenterologist", "Dentist", "Neurologist", "Dermatologist" };

            int id = 1;

            var faker = new Faker<Specialty>()
                .RuleFor(s => s.SpecialtyName, f => specialties[id - 1])
                .RuleFor(s => s.SpecialtyId, f => id++);

            var testSpecialties = faker.Generate(specialties.Length);

            return testSpecialties;
        }

        public static List<AppUser> GetTestAppUsers(int count = 100)
        {
            var levels = new[] { true, false };

            var faker = new Faker<AppUser>()
                .RuleFor(u => u.Id, f => f.UniqueIndex.ToString())
                .RuleFor(u => u.UserName, f => f.Internet.Email())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.Name, f => f.Name.FirstName())
                .RuleFor(u => u.Surname, f => f.Name.LastName());

            var testUsers = faker.Generate(count);

            return testUsers;
        }

        public static List<Admin> GetTestAdmins(int count = 100)
        {
            var levels = new[] { true, false };

            var faker = new Faker<Admin>()
                .RuleFor(u => u.UserName, f => f.Internet.Email())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.Name, f => f.Name.FirstName())
                .RuleFor(u => u.Surname, f => f.Name.LastName())
                .RuleFor(a => a.IsSuperAdmin, f => f.PickRandom(levels));

            var testAdmins = faker.Generate(count);

            return testAdmins;
        }

        public static List<Admin> GetTestAdminsWithId(int count = 100)
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

            var testAdmins = faker.Generate(count);

            return testAdmins;
        }

        public static List<Doctor> GetTestDoctors(int count = 100)
        {
            var faker = new Faker<Doctor>()
                .RuleFor(u => u.Id, f => f.UniqueIndex.ToString())
                .RuleFor(u => u.UserName, f => f.Internet.Email())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.Name, f => f.Name.FirstName())
                .RuleFor(u => u.Surname, f => f.Name.LastName());

            var testDoctors = faker.Generate(count);

            return testDoctors;
        }

        public static List<Doctor> GetTestDoctors(
            IEnumerable<Specialty> specialties,
            IEnumerable<Hospital> hospitals,
            int count = 100)
        {
            var faker = new Faker<Doctor>()
                .RuleFor(u => u.UserName, f => f.Internet.Email())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.Name, f => f.Name.FirstName())
                .RuleFor(u => u.Surname, f => f.Name.LastName())
                .RuleFor(d => d.Specialty, f => f.PickRandom(specialties))
                .RuleFor(d => d.Hospital, f => f.PickRandom(hospitals));

            var testDoctors = faker.Generate(count);

            return testDoctors;
        }

        public static List<Doctor> GetTestDoctorsWithId(
            IEnumerable<Specialty> specialties,
            IEnumerable<Hospital> hospitals,
            int count = 100)
        {
            var faker = new Faker<Doctor>()
                .RuleFor(u => u.Id, f => f.UniqueIndex.ToString())
                .RuleFor(u => u.UserName, f => f.Internet.Email())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.Name, f => f.Name.FirstName())
                .RuleFor(u => u.Surname, f => f.Name.LastName())
                .RuleFor(d => d.Specialty, f => f.PickRandom(specialties))
                .RuleFor(d => d.Hospital, f => f.PickRandom(hospitals));

            var testDoctors = faker.Generate(count);

            return testDoctors;
        }

        public static List<Patient> GetTestPatients(int count = 100)
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

            var testPatients = faker.Generate(count);

            return testPatients;
        }

        public static List<Patient> GetTestPatients(IEnumerable<Address> addresses, int count = 100)
        {
            var sexes = new[] { Sex.Male, Sex.Female };

            var faker = new Faker<Patient>()
                .RuleFor(u => u.UserName, f => f.Internet.Email())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.Name, f => f.Name.FirstName())
                .RuleFor(u => u.Surname, f => f.Name.LastName())
                .RuleFor(p => p.Sex, f => f.PickRandom(sexes))
                .RuleFor(p => p.BirthDate, f => f.Date.Past(70, new DateTime(2000, 1, 1)))
                .RuleFor(p => p.Address, f => f.PickRandom(addresses));

            var testPatients = faker.Generate(count);

            return testPatients;
        }

        public static List<Patient> GetTestPatientsWithId(IEnumerable<Address> addresses, int count = 100)
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

            var testPatients = faker.Generate(count);

            return testPatients;
        }
    }
}
