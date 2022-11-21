﻿using HospitalWeb.DAL.Entities;
using HospitalWeb.WebApi.Clients.Interfaces;
using HospitalWeb.WebApi.Models.ResourceModels;

namespace HospitalWeb.WebApi.Clients.Implementations
{
    public class DiagnosesApiClient : ApiClient<Diagnosis, DiagnosisResourceModel, int>
    {
        public DiagnosesApiClient(IConfiguration config) : base(config)
        {
        }

        public override HttpResponseMessage Get()
        {
            return _client.GetAsync("Diagnoses").Result;
        }

        public override HttpResponseMessage Get(int identifier)
        {
            return _client.GetAsync($"Diagnoses/{identifier}").Result;
        }

        public HttpResponseMessage Get(string name)
        {
            return _client.GetAsync($"Diagnoses/details?name={name}").Result;
        }

        public Diagnosis GetOrCreate(string name)
        {
            var response = Get(name);

            if (response.IsSuccessStatusCode)
            {
                return Read(response);
            }
            else
            {
                var diagnosis = new DiagnosisResourceModel
                {
                    DiagnosisName = name
                };

                return Read(Post(diagnosis));
            }
        }

        public override Diagnosis Read(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<Diagnosis>().Result;
        }

        public override Diagnosis Read(int identifier)
        {
            var response = Get(identifier);
            return Read(response);
        }

        public override IEnumerable<Diagnosis> ReadMany(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<IEnumerable<Diagnosis>>().Result;
        }

        public override HttpResponseMessage Post(DiagnosisResourceModel obj)
        {
            return _client.PostAsJsonAsync("Diagnoses", obj).Result;
        }

        public override HttpResponseMessage Put(DiagnosisResourceModel obj)
        {
            return _client.PutAsJsonAsync("Diagnoses", obj).Result;
        }

        public override HttpResponseMessage Delete(int identifier)
        {
            return _client.DeleteAsync($"Diagnoses/{identifier}").Result;
        }
    }
}
