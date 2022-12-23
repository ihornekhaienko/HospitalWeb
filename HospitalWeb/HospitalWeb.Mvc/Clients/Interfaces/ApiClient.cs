namespace HospitalWeb.Mvc.Clients.Interfaces
{
    public abstract class ApiClient<TEntity, TResource, TIdentifier> : IApiClient<TEntity, TResource, TIdentifier>
    {
        protected readonly IConfiguration _config;
        protected readonly HttpClient _client;
        protected readonly IHttpClientFactory _clientFactory;
        protected readonly string _addressSuffix;

        public ApiClient(IConfiguration config, IHttpClientFactory clientFactory, string addressSuffix)
        {
            _config = config;

            _clientFactory = clientFactory;
            _addressSuffix = addressSuffix;
            _client = _clientFactory.CreateClient(_addressSuffix);
            _client.BaseAddress = new Uri(_config["WebApi:Url"]);
        }

        public abstract HttpResponseMessage Get(string token = null, string provider = null);

        public abstract HttpResponseMessage Get(TIdentifier identifier, string token = null, string provider = null);

        public abstract TEntity Read(HttpResponseMessage response);

        public abstract TEntity Read(TIdentifier identifier);

        public abstract IEnumerable<TEntity> ReadMany(HttpResponseMessage response);

        public abstract TErrorResponse ReadError<TErrorResponse>(HttpResponseMessage response);

        public abstract HttpResponseMessage Post(TResource obj, string token = null, string provider = null);

        public abstract HttpResponseMessage Post(TEntity obj, string token = null, string provider = null);

        public abstract HttpResponseMessage Put(TEntity obj, string token = null, string provider = null);

        public abstract HttpResponseMessage Delete(TIdentifier identifier, string token = null, string provider = null);
    }
}
