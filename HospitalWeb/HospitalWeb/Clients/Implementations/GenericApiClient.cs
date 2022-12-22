using AutoMapper;
using HospitalWeb.Clients.Interfaces;
using System.Globalization;
using System.Net.Http.Headers;

namespace HospitalWeb.Clients.Implementations
{
    public class GenericApiClient<TEntity, TResource, TIdentifier> : ApiClient<TEntity, TResource, TIdentifier>
    {
        public GenericApiClient(IConfiguration config, IHttpClientFactory clientFactory, string addressSuffix) 
            : base(config, clientFactory, addressSuffix)
        {
        }

        public override HttpResponseMessage Get(string token = null, string provider = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_client.BaseAddress}{_addressSuffix}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("Provider", provider);
            request.Headers.Add("Accept-Language", CultureInfo.CurrentCulture.Name);

            return _client.SendAsync(request).Result;
        }

        public override HttpResponseMessage Get(TIdentifier identifier, string token = null, string provider = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_client.BaseAddress}{_addressSuffix}/{identifier}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("Provider", provider);
            request.Headers.Add("Accept-Language", CultureInfo.CurrentCulture.Name);

            return _client.SendAsync(request).Result;
        }

        public override HttpResponseMessage Post(TResource obj, string token = null, string provider = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_client.BaseAddress}{_addressSuffix}");
            HttpContent httpContent = JsonContent.Create(obj);

            request.Content = httpContent;
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("Provider", provider);
            request.Headers.Add("Accept-Language", CultureInfo.CurrentCulture.Name);

            return _client.SendAsync(request).Result;
        }

        public override HttpResponseMessage Post(TEntity obj, string token = null, string provider = null)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<TEntity, TResource>());
            var mapper = new Mapper(config);
            var model = mapper.Map<TEntity, TResource>(obj);

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_client.BaseAddress}{_addressSuffix}");
            HttpContent httpContent = JsonContent.Create(obj);

            request.Content = httpContent;
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("Provider", provider);
            request.Headers.Add("Accept-Language", CultureInfo.CurrentCulture.Name);

            return _client.SendAsync(request).Result;
        }

        public override HttpResponseMessage Put(TEntity obj, string token = null, string provider = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"{_client.BaseAddress}{_addressSuffix}");
            HttpContent httpContent = JsonContent.Create(obj);

            request.Content = httpContent;
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("Provider", provider);
            request.Headers.Add("Accept-Language", CultureInfo.CurrentCulture.Name);

            return _client.SendAsync(request).Result;
        }

        public override HttpResponseMessage Delete(TIdentifier identifier, string token = null, string provider = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{_client.BaseAddress}{_addressSuffix}/{identifier}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("Provider", provider);
            request.Headers.Add("Accept-Language", CultureInfo.CurrentCulture.Name);

            return _client.SendAsync(request).Result;
        }

        public override TEntity Read(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<TEntity>().Result;
        }

        public override TEntity Read(TIdentifier identifier)
        {
            var response = Get(identifier);
            return Read(response);
        }

        public override IEnumerable<TEntity> ReadMany(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<IEnumerable<TEntity>>().Result;
        }

        public override TErrorResponse ReadError<TErrorResponse>(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<TErrorResponse>().Result;
        }
    }
}
