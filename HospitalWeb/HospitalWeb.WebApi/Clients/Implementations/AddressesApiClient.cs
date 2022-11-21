using AutoMapper;
using HospitalWeb.DAL.Entities;
using HospitalWeb.WebApi.Clients.Interfaces;
using HospitalWeb.WebApi.Models.ResourceModels;

namespace HospitalWeb.WebApi.Clients.Implementations
{
    public class AddressesApiClient : ApiClient<Address, AddressResourceModel, int>
    {
        public AddressesApiClient(IConfiguration config) : base(config)
        {
        }

        public override HttpResponseMessage Get()
        {
            return _client.GetAsync("Addresses").Result;
        }

        public override HttpResponseMessage Get(int identifier)
        {
            return _client.GetAsync($"Addresses/{identifier}").Result;
        }

        public HttpResponseMessage Get(string address, string locality)
        {
            return _client.GetAsync($"Addresses/details?address={address}&locality={locality}").Result;
        }

        public Address GetOrCreate(string address, Locality locality)
        {
            var response = Get(address, locality.LocalityName);

            if (response.IsSuccessStatusCode)
            {
                return Read(response);
            }
            else
            {
                var obj = new AddressResourceModel
                {
                    FullAddress = address,
                    LocalityId = locality.LocalityId
                };

                return Read(Post(obj));
            }
        }

        public override Address Read(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<Address>().Result;
        }

        public override Address Read(int identifier)
        {
            var response = Get(identifier);
            return Read(response);
        }

        public override IEnumerable<Address> ReadMany(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<IEnumerable<Address>>().Result;
        }

        public override HttpResponseMessage Post(AddressResourceModel obj)
        {
            return _client.PostAsJsonAsync("Addresses", obj).Result;
        }

        public override HttpResponseMessage Post(Address obj)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Address, AddressResourceModel>());
            var mapper = new Mapper(config);

            var model = mapper.Map<Address, AddressResourceModel>(obj);

            return Post(model);
        }

        public override HttpResponseMessage Put(AddressResourceModel obj)
        {
            return _client.PutAsJsonAsync("Addresses", obj).Result;
        }

        public override HttpResponseMessage Put(Address obj)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Address, AddressResourceModel>());
            var mapper = new Mapper(config);

            var model = mapper.Map<Address, AddressResourceModel>(obj);

            return Put(model);
        }

        public override HttpResponseMessage Delete(int identifier)
        {
            return _client.DeleteAsync($"Addresses/{identifier}").Result;
        }
    }
}
