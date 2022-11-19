namespace HospitalWeb.WebApi.Clients.Interfaces
{
    public interface IApiClient<TType, TIdentifier>
    {
        public HttpResponseMessage Get();

        public HttpResponseMessage Get(TIdentifier identifier);

        public TType Read(HttpResponseMessage response);

        public TType Read(TIdentifier identifier);

        public IEnumerable<TType> ReadMany(HttpResponseMessage response);

        public HttpResponseMessage Post(TType obj);

        public HttpResponseMessage Put(TType obj);

        public HttpResponseMessage Delete(TType obj);

        public HttpResponseMessage Delete(TIdentifier identifier);
    }
}
