using HospitalWeb.DAL.Entities;
using HospitalWeb.WebApi.Models.ResourceModels;
using System.Globalization;
using System.Net.Http.Headers;

namespace HospitalWeb.Clients.Implementations
{
    public class DiagnosesApiClient : GenericApiClient<Diagnosis, DiagnosisResourceModel, int>
    {
        public DiagnosesApiClient(IConfiguration config, IHttpClientFactory clientFactory) 
            : base(config, clientFactory, "Diagnoses")
        {
        }

        public HttpResponseMessage Get(string name, string token = null, string provider = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_client.BaseAddress}{_addressSuffix}/details?name={name}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("Provider", provider);
            request.Headers.Add("Accept-Language", CultureInfo.CurrentCulture.Name);

            return _client.SendAsync(request).Result;
        }

        public Diagnosis GetOrCreate(string name, string token = null, string provider = null)
        {
            var response = Get(name, token, provider);

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

                return Read(Post(diagnosis, token, provider));
            }
        }
    }
}
