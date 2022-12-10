using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Entities.Identity;

namespace HospitalWeb.DAL.Services.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<AppUser> AppUsers { get; }
        IRepository<Address> Addresses { get; }
        IRepository<Admin> Admins { get; }
        IRepository<Appointment> Appointments { get; }
        IRepository<Diagnosis> Diagnoses { get; }
        IRepository<Doctor> Doctors { get; }
        IRepository<Hospital> Hospitals { get; }
        IRepository<Locality> Localities { get; }
        IRepository<Meeting> Meetings { get; }
        IRepository<Notification> Notifications { get; }
        IRepository<Patient> Patients { get; }
        IRepository<Schedule> Schedules { get; }
        IRepository<Specialty> Specialties { get; }

        void Detach();
    }
}
