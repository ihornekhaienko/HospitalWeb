using HospitalWeb.DAL.Entities;
using HospitalWeb.WebApi.Models.ResourceModels;
using System.Net.Http.Headers;

namespace HospitalWeb.Clients.Implementations
{
    public class GradesApiClient : GenericApiClient<Grade, GradeResourceModel, int>
    {
        public GradesApiClient(IConfiguration config, IHttpClientFactory clientFactory)
            : base(config, clientFactory, "Grades")
        {
        }

        public HttpResponseMessage Filter(
            string author = null, 
            string target = null,
            string token = null,
            string provider = null)
        {
            string query = $"?author={author}&target={target}";

            var request = new HttpRequestMessage(HttpMethod.Get, $"{_client.BaseAddress}{_addressSuffix}{query}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("Provider", provider);

            return _client.SendAsync(request).Result;
        }
    }
}
