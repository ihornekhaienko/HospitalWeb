namespace HospitalWeb.WebApi.Clients.Interfaces
{
    public abstract class ApiClient<TType, TIdentifier> : IApiClient<TType, TIdentifier>
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

        public abstract TType Read(HttpResponseMessage response);

        public abstract HttpResponseMessage Post(TType obj);

        public abstract HttpResponseMessage Put(TType obj);

        public abstract HttpResponseMessage Delete(TType obj);

        public abstract HttpResponseMessage Delete(TIdentifier identifier);
    }
}
