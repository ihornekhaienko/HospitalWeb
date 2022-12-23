using HospitalWeb.Domain.Entities;
using HospitalWeb.Mvc.Models.ResourceModels;
using System.Globalization;
using System.Net.Http.Headers;

namespace HospitalWeb.Mvc.Clients.Implementations
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
            string query = $"/details?author={author}&target={target}";

            var request = new HttpRequestMessage(HttpMethod.Get, $"{_client.BaseAddress}{_addressSuffix}{query}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("Provider", provider);
            request.Headers.Add("Accept-Language", CultureInfo.CurrentCulture.Name);

            return _client.SendAsync(request).Result;
        }

        public HttpResponseMessage AddOrUpdate(
            int stars,
            string author, 
            string target,
            string token = null,
            string provider = null)
        {
            var response = Filter(author, target, token, provider);

            if (response.IsSuccessStatusCode)
            {
                var grade = ReadMany(response).FirstOrDefault();

                if (grade != null)
                {
                    grade.Stars = stars;
                    return Put(grade);
                }
            }

            var gradeResource = new GradeResourceModel
            {
                Stars = stars,
                AuthorId = author,
                TargetId = target
            };

            return Post(gradeResource);
        }
    }
}
