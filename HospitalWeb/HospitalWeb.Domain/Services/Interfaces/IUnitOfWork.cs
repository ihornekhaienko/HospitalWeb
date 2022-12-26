using HospitalWeb.Domain.Entities;
using HospitalWeb.Domain.Entities.Identity;

namespace HospitalWeb.Domain.Services.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<AppUser> AppUsers { get; }
        IRepository<Address> Addresses { get; }
        IRepository<Admin> Admins { get; }
        IRepository<Appointment> Appointments { get; }
        IRepository<Diagnosis> Diagnoses { get; }
        IRepository<Doctor> Doctors { get; }
        IRepository<Grade> Grades { get; }
        IRepository<Hospital> Hospitals { get; }
        IRepository<Locality> Localities { get; }
        IRepository<Meeting> Meetings { get; }
        IRepository<Message> Messages { get; }
        IRepository<Notification> Notifications { get; }
        IRepository<Patient> Patients { get; }
        IRepository<Schedule> Schedules { get; }
        IRepository<Specialty> Specialties { get; }

        void Detach();
    }
}
