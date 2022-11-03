using HospitalWeb.DAL.Entities;

namespace HospitalWeb.DAL.Services.Interfaces
{
    public interface IAddressService
    {
        IEnumerable<Address>? GetAll();
        Address? Get(int id);
        Task<Address?> Create(string? address, Locality? locality);
        Task Update(Address address);
        Task Delete(Address address);
    }
}
