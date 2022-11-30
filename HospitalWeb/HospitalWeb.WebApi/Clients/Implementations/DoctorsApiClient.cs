﻿using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.WebApi.Models.ResourceModels;
using HospitalWeb.WebApi.Models.SortStates;
using System.Net.Http.Headers;

namespace HospitalWeb.WebApi.Clients.Implementations
{
    public class DoctorsApiClient : GenericApiClient<Doctor, DoctorResourceModel, string>
    {
        public DoctorsApiClient(IConfiguration config) : base(config, "Doctors")
        {
        }

        public HttpResponseMessage Filter(
            string searchString,
            int? specialty,
            DoctorSortState sortOrder = DoctorSortState.Id,
            int pageSize = 10,
            int pageNumber = 1,
            string token = null,
            string provider = null)
        {
            string query = $"?searchString={searchString}&specialty={specialty}&sortOrder={sortOrder}" +
                $"&pageSize={pageSize}&pageNumber={pageNumber}";

            var request = new HttpRequestMessage(HttpMethod.Get, $"{_client.BaseAddress}{_addressSuffix}{query}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("Provider", provider);

            return _client.SendAsync(request).Result;
        }
    }
}
