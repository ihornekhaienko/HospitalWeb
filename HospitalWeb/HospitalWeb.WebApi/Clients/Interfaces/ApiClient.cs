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

        public abstract HttpResponseMessage Get();

        public abstract HttpResponseMessage Get(TIdentifier identifier);

        public abstract TEntity Read(HttpResponseMessage response);

        public abstract TEntity Read(TIdentifier identifier);

        public abstract IEnumerable<TEntity> ReadMany(HttpResponseMessage response);

        public abstract HttpResponseMessage Post(TResource obj);

        public abstract HttpResponseMessage Post(TEntity obj);

        public abstract HttpResponseMessage Put(TResource obj);

        public abstract HttpResponseMessage Put(TEntity obj);

        public abstract HttpResponseMessage Delete(TIdentifier identifier);
    }
}
