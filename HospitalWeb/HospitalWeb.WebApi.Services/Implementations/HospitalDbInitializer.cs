using HospitalWeb.Domain.Entities;
using HospitalWeb.Domain.Entities.Identity;
using HospitalWeb.Domain.Services.Interfaces;
using HospitalWeb.WebApi.Services.Extensions;
using HospitalWeb.WebApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace HospitalWeb.WebApi.Services.Implementations
{
    public class HospitalDbInitializer : IDbInitializer
    {
        private readonly IConfiguration _config;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _uow;
        private readonly bool _fullGeneration;

        public HospitalDbInitializer(
            IConfiguration config,
            UserManager<AppUser> userManager, 
            RoleManager<IdentityRole> roleManager, 
            IUnitOfWork uow,
            bool fullGeneration)
        {
            _config = config;
            _userManager = userManager;
            _roleManager = roleManager;
            _uow = uow;
            _fullGeneration = fullGeneration;
        }

        public async Task Init()
        {
            await SetupRoles();
            await CreateSuperAdmin();

            if (_fullGeneration)
            {
                await GenerateDb();
            }
        }

        public async Task SetupRoles()
        {
            if (await _roleManager.FindByNameAsync("Admin") == null)
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (await _roleManager.FindByNameAsync("Patient") == null)
            {
                await _roleManager.CreateAsync(new IdentityRole("Patient"));
            }
            if (await _roleManager.FindByNameAsync("Doctor") == null)
            {
                await _roleManager.CreateAsync(new IdentityRole("Doctor"));
            }
        }

        public async Task CreateSuperAdmin()
        {
            if (await _userManager.FindByEmailAsync(_config["SuperAdmin:Email"]) == null)
            {
                string email = _config["SuperAdmin:Email"];
                string password = _config["SuperAdmin:Password"];

                var admin = new Admin
                {
                    Name = _config["SuperAdmin:Name"],
                    Surname = _config["SuperAdmin:Surname"],
                    UserName = email,
                    Email = email,
                    PhoneNumber = _config["SuperAdmin:Phone"],
                    EmailConfirmed = true,
                    IsSuperAdmin = true
                };

                var result = await _userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }

        public async Task GenerateDb()
        {
            var localities = await GenerateLocalities();

            var addresses = await GenerateAddresses(localities);

            var diagnoses = await GenerateDiagnoses();

            var hospitals = await GenerateHospitals(addresses);

            var specialties = await GenerateSpecialties();

            var admins = await GenerateAdmins();

            var doctors = await GenerateDoctors(specialties, hospitals);

            var patients = await GeneratePatients(addresses);

            var appointments = await GenerateAppointments(diagnoses, doctors, patients);
        }

        private async Task<IEnumerable<Address>> GenerateAddresses(IEnumerable<Locality> localities)
        {
            if (_uow.Addresses.Count != 0)
            {
                return await _uow.Addresses.GetAllAsync();
            }

            var addresses = DataGenerator.GetTestAddresses(localities);
            await _uow.Addresses.CreateManyAsync(addresses);

            return addresses;
        }

        private async Task<IEnumerable<Diagnosis>> GenerateDiagnoses()
        {
            if (_uow.Diagnoses.Count != 0)
            {
                return await _uow.Diagnoses.GetAllAsync();
            }

            var diagnoses = DataGenerator.GetTestDiagnoses(count: 15);
            await _uow.Diagnoses.CreateManyAsync(diagnoses);

            return diagnoses;
        }

        private async Task<IEnumerable<Hospital>> GenerateHospitals(IEnumerable<Address> addresses)
        {
            if (_uow.Hospitals.Count != 0)
            {
                return await _uow.Hospitals.GetAllAsync();
            }

            var hospitals = DataGenerator.GetTestHospitals(addresses, count: 10);
            await _uow.Hospitals.CreateManyAsync(hospitals);

            return hospitals;
        }

        private async Task<IEnumerable<Locality>> GenerateLocalities()
        {
            if (_uow.Localities.Count != 0)
            {
                return await _uow.Localities.GetAllAsync();
            }

            var localities = DataGenerator.GetTestLocalities();
            await _uow.Localities.CreateManyAsync(localities);

            return localities;
        }

        private async Task<IEnumerable<Specialty>> GenerateSpecialties()
        {
            if (_uow.Specialties.Count != 0)
            {
                return await _uow.Specialties.GetAllAsync();
            }

            var specialties = DataGenerator.GetTestSpecialties();
            await _uow.Specialties.CreateManyAsync(specialties);

            return specialties;
        }

        private async Task<IEnumerable<Admin>> GenerateAdmins()
        {
            if (_uow.Admins.Count != 0)
            {
                return await _uow.Admins.GetAllAsync();
            }

            var admins = DataGenerator.GetTestAdmins(count: 10);
            await _uow.Admins.CreateManyAsync(admins);

            return admins;
        }

        private async Task<IEnumerable<Doctor>> GenerateDoctors(IEnumerable<Specialty> specialties, IEnumerable<Hospital> hospitals)
        {
            if (_uow.Doctors.Count != 0)
            {
                return await _uow.Doctors.GetAllAsync();
            }

            var doctors = DataGenerator.GetTestDoctors(specialties, hospitals);
            await _uow.Doctors.CreateManyAsync(doctors);

            return doctors;
        }

        private async Task<IEnumerable<Patient>> GeneratePatients(IEnumerable<Address> addresses)
        {
            if (_uow.Patients.Count != 0)
            {
                return await _uow.Patients.GetAllAsync();
            }

            var patients = DataGenerator.GetTestPatients(addresses);
            await _uow.Patients.CreateManyAsync(patients);

            return patients;
        }

        private async Task<IEnumerable<Appointment>> GenerateAppointments(
            IEnumerable<Diagnosis> diagnoses,
            IEnumerable<Doctor> doctors, 
            IEnumerable<Patient> patients)
        {
            if (_uow.Appointments.Count != 0)
            {
                return await _uow.Appointments.GetAllAsync();
            }

            var appointments = DataGenerator.GetTestAppointments(diagnoses, doctors, patients, 500);
            await _uow.Appointments.CreateManyAsync(appointments);

            return appointments;
        }
    }
}
