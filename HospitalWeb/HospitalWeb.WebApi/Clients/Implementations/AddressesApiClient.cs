﻿using HospitalWeb.DAL.Entities;
using HospitalWeb.WebApi.Clients.Interfaces;

namespace HospitalWeb.WebApi.Clients.Implementations
{
    public class AddressesApiClient : ApiClient<Address, int>
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
                var obj = new Address
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

        public override HttpResponseMessage Post(Address obj)
        {
            return _client.PostAsJsonAsync("Addresses", obj).Result;
        }

        public override HttpResponseMessage Put(Address obj)
        {
            return _client.PutAsJsonAsync("Addresses", obj).Result;
        }

        public override HttpResponseMessage Delete(Address obj)
        {
            return _client.DeleteAsync($"Addresses/{obj}").Result;
        }

        public override HttpResponseMessage Delete(int identifier)
        {
            return _client.DeleteAsync($"Addresses/{identifier}").Result;
        }
    }
}