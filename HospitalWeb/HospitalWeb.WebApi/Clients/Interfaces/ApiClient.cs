namespace HospitalWeb.WebApi.Clients.Interfaces
{
    public abstract class ApiClient<TEntity, TResource, TIdentifier> : IApiClient<TEntity, TResource, TIdentifier>
    {
        protected readonly HttpClient _client;
        protected readonly IConfiguration _config;

        public ApiClient(IConfiguration config)
        {
            _client = new HttpClient();
            _config = config;
            _client.BaseAddress = new Uri(_config["WebApi:Url"]);
        }

        public abstract HttpResponseMessage Get(string token = null, string provider = null);

        public abstract HttpResponseMessage Get(TIdentifier identifier, string token = null, string provider = null);

        public abstract TEntity Read(HttpResponseMessage response);

        public abstract TEntity Read(TIdentifier identifier);

        public abstract IEnumerable<TEntity> ReadMany(HttpResponseMessage response);

        public abstract HttpResponseMessage Post(TResource obj, string token = null, string provider = null);

        public abstract HttpResponseMessage Post(TEntity obj, string token = null, string provider = null);

        public abstract HttpResponseMessage Put(TEntity obj, string token = null, string provider = null);

        public abstract HttpResponseMessage Delete(TIdentifier identifier, string token = null, string provider = null);
    }
}
