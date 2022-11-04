using HospitalWeb.DAL.Entities;

namespace HospitalWeb.DAL.Services.Interfaces
{
    public interface ISpecialtyService
    {
        IEnumerable<Specialty>? GetAll();
        Specialty? Get(int id);
        Task<Specialty?> Create(string? name);
        Task Update(Specialty specialty);
        Task Delete(Specialty specialty);
    }
}
