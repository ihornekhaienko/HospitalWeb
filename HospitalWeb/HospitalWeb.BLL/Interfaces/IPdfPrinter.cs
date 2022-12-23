using HospitalWeb.Domain.Entities;

namespace HospitalWeb.Services.Interfaces
{
    public interface IPdfPrinter
    {
        public void PrintAppointment(Appointment appointment, string filePath);
    }
}
