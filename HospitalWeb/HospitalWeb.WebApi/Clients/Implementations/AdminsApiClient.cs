using AutoMapper;
using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.WebApi.Clients.Interfaces;
using HospitalWeb.WebApi.Models.ResourceModels;
using HospitalWeb.WebApi.Models.SortStates;
using Microsoft.AspNetCore.Identity;

namespace HospitalWeb.WebApi.Clients.Implementations
{
    public class AdminsApiClient : ApiClient<Admin, AdminResourceModel, string>
    {
        public AdminsApiClient(IConfiguration config) : base(config)
        {
        }

        public override HttpResponseMessage Get()
        {
            return _client.GetAsync("Admins").Result;
        }

        public HttpResponseMessage Filter(
            string searchString,
            AdminSortState sortOrder = AdminSortState.Id,
            int pageSize = 10,
            int pageNumber = 1)
        {
            return _client.GetAsync($"Admins?searchString={searchString}&sortOrder={sortOrder}&pageSize={pageSize}&pageNumber={pageNumber}").Result;
        }

        public override HttpResponseMessage Get(string identifier)
        {
            return _client.GetAsync($"Admins/{identifier}").Result;
        }

        public override Admin Read(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<Admin>().Result;
        }

        public override Admin Read(string identifier)
        {
            var response = Get(identifier);
            return Read(response);
        }

        public override IEnumerable<Admin> ReadMany(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<IEnumerable<Admin>>().Result;
        }

        public IEnumerable<IdentityError> ReadErrors(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<IEnumerable<IdentityError>>().Result;
        }

        public override HttpResponseMessage Post(AdminResourceModel obj)
        {
            return _client.PostAsJsonAsync("Admins", obj).Result;
        }

        public override HttpResponseMessage Post(Admin obj)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Admin, AdminResourceModel>());
            var mapper = new Mapper(config);

            var model = mapper.Map<Admin, AdminResourceModel>(obj);

            return Post(model);
        }

        public override HttpResponseMessage Put(AdminResourceModel obj)
        {
            return _client.PutAsJsonAsync("Admins", obj).Result;
        }

        public override HttpResponseMessage Put(Admin obj)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Admin, AdminResourceModel>());
            var mapper = new Mapper(config);

            var model = mapper.Map<Admin, AdminResourceModel>(obj);

            return Put(model);
        }

        public override HttpResponseMessage Delete(string identifier)
        {
            return _client.DeleteAsync($"Admins/{identifier}").Result;
        }
    }
}
