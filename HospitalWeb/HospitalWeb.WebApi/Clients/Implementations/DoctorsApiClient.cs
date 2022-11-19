using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.WebApi.Clients.Interfaces;
using HospitalWeb.WebApi.Models.ResourceModels;
using HospitalWeb.WebApi.Models.SortStates;
using Microsoft.AspNetCore.Identity;

namespace HospitalWeb.WebApi.Clients.Implementations
{
    public class DoctorsApiClient : ApiClient<Doctor, string>
    {
        public DoctorsApiClient(IConfiguration config) : base(config)
        {
        }

        public override HttpResponseMessage Get()
        {
            return _client.GetAsync("Doctors").Result;
        }

        public HttpResponseMessage Filter(
            string searchString,
            int? specialty,
            DoctorSortState sortOrder = DoctorSortState.Id,
            int pageSize = 10,
            int pageNumber = 1)
        {
            return _client.GetAsync($"Doctors?searchString={searchString}&specialty={specialty}&sortOrder={sortOrder}" +
                $"&pageSize={pageSize}&pageNumber={pageNumber}").Result;
        }

        public override HttpResponseMessage Get(string identifier)
        {
            return _client.GetAsync($"Doctors/{identifier}").Result;
        }

        public override Doctor Read(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<Doctor>().Result;
        }

        public override Doctor Read(string identifier)
        {
            var response = Get(identifier);
            return Read(response);
        }

        public override IEnumerable<Doctor> ReadMany(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<IEnumerable<Doctor>>().Result;
        }

        public IEnumerable<IdentityError> ReadErrors(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<IEnumerable<IdentityError>>().Result;
        }

        public override HttpResponseMessage Post(Doctor obj)
        {
            var model = new DoctorResourceModel
            {
                Name = obj.Name,
                Surname = obj.Surname,
                Email = obj.Email,
                UserName = obj.Email,
                PhoneNumber = obj.PhoneNumber,
                SpecialtyId = obj.SpecialtyId,
                EmailConfirmed = obj.EmailConfirmed
            };

            return _client.PostAsJsonAsync("Doctors", model).Result;
        }

        public HttpResponseMessage Post(DoctorResourceModel obj)
        {
            return _client.PostAsJsonAsync("Doctors", obj).Result;
        }

        public override HttpResponseMessage Put(Doctor obj)
        {
            return _client.PutAsJsonAsync("Doctors", obj).Result;
        }

        public override HttpResponseMessage Delete(Doctor obj)
        {
            return _client.DeleteAsync($"Doctors/{obj}").Result;
        }

        public override HttpResponseMessage Delete(string identifier)
        {
            return _client.DeleteAsync($"Doctors/{identifier}").Result;
        }
    }
}
