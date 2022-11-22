using AutoMapper;
using HospitalWeb.DAL.Entities;
using HospitalWeb.WebApi.Clients.Interfaces;
using HospitalWeb.WebApi.Models.ResourceModels;

namespace HospitalWeb.WebApi.Clients.Implementations
{
    public class SpecialtiesApiClient : ApiClient<Specialty, SpecialtyResourceModel, int>
    {
        public SpecialtiesApiClient(IConfiguration config) : base(config)
        {
        }

        public override HttpResponseMessage Get()
        {
            return _client.GetAsync("Specialties").Result;
        }

        public override HttpResponseMessage Get(int identifier)
        {
            return _client.GetAsync($"Specialties/{identifier}").Result;
        }

        public HttpResponseMessage Get(string name)
        {
            return _client.GetAsync($"Specialties/details?name={name}").Result;
        }

        public Specialty GetOrCreate(string name)
        {
            var response = Get(name);

            if (response.IsSuccessStatusCode)
            {
                return Read(response);
            }
            else
            {
                var specialty = new SpecialtyResourceModel
                {
                    SpecialtyName = name
                };

                return Read(Post(specialty));
            }
        }

        public override Specialty Read(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<Specialty>().Result;
        }

        public override Specialty Read(int identifier)
        {
            var response = Get(identifier);
            return Read(response);
        }

        public override IEnumerable<Specialty> ReadMany(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<IEnumerable<Specialty>>().Result;
        }

        public override HttpResponseMessage Post(SpecialtyResourceModel obj)
        {
            return _client.PostAsJsonAsync("Specialties", obj).Result;
        }

        public override HttpResponseMessage Post(Specialty obj)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Specialty, SpecialtyResourceModel>());
            var mapper = new Mapper(config);

            var model = mapper.Map<Specialty, SpecialtyResourceModel>(obj);

            return Post(model);
        }

        public override HttpResponseMessage Put(Specialty obj)
        {
            return _client.PutAsJsonAsync("Specialties", obj).Result;
        }

        public override HttpResponseMessage Delete(int identifier)
        {
            return _client.DeleteAsync($"Specialties/{identifier}").Result;
        }
    }
}
