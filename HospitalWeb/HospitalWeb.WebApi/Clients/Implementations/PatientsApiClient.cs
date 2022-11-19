﻿using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.WebApi.Clients.Interfaces;
using HospitalWeb.WebApi.Models.ResourceModels;
using HospitalWeb.WebApi.Models.SortStates;
using Microsoft.AspNetCore.Identity;

namespace HospitalWeb.WebApi.Clients.Implementations
{
    public class PatientsApiClient : ApiClient<Patient, string>
    {
        public PatientsApiClient(IConfiguration config) : base(config)
        {
        }

        public override HttpResponseMessage Get()
        {
            return _client.GetAsync("Patients").Result;
        }

        public HttpResponseMessage Filter(
            string searchString,
            int? locality,
            PatientSortState sortOrder = PatientSortState.Id,
            int pageSize = 10,
            int pageNumber = 1)
        {
            return _client.GetAsync($"Patients?searchString={searchString}&locality={locality}&sortOrder={sortOrder}" +
                $"&pageSize={pageSize}&pageNumber={pageNumber}").Result;
        }

        public override HttpResponseMessage Get(string identifier)
        {
            return _client.GetAsync($"Patients/{identifier}").Result;
        }

        public override Patient Read(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<Patient>().Result;
        }

        public override Patient Read(string identifier)
        {
            var response = Get(identifier);
            return Read(response);
        }

        public override IEnumerable<Patient> ReadMany(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<IEnumerable<Patient>>().Result;
        }

        public IEnumerable<IdentityError> ReadErrors(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<IEnumerable<IdentityError>>().Result;
        }

        public override HttpResponseMessage Post(Patient obj)
        {
            var model = new PatientResourceModel
            {
                Name = obj.Name,
                Surname = obj.Surname,
                UserName = obj.Email,
                Email = obj.Email,
                PhoneNumber = obj.PhoneNumber,
                AddressId = obj.AddressId,
                BirthDate = obj.BirthDate,
                Sex = obj.Sex
            };

            return _client.PostAsJsonAsync("Patients", model).Result;
        }

        public HttpResponseMessage Post(PatientResourceModel obj)
        {
            return _client.PostAsJsonAsync("Patients", obj).Result;
        }

        public override HttpResponseMessage Put(Patient obj)
        {
            return _client.PutAsJsonAsync("Patients", obj).Result;
        }

        public override HttpResponseMessage Delete(Patient obj)
        {
            return _client.DeleteAsync($"Patients/{obj}").Result;
        }

        public override HttpResponseMessage Delete(string identifier)
        {
            return _client.DeleteAsync($"Patients/{identifier}").Result;
        }
    }
}