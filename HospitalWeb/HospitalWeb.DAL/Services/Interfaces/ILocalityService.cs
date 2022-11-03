using HospitalWeb.DAL.Entities;

namespace HospitalWeb.DAL.Services.Interfaces
{
    public interface ILocalityService
    {
        IEnumerable<Locality>? GetAll();
        Locality? Get(int id);
        Task<Locality?> Create(string? locality);
        Task Update(Locality locality);
        Task Delete(Locality locality);
    }
}
