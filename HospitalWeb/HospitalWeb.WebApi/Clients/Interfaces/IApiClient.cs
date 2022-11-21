namespace HospitalWeb.WebApi.Clients.Interfaces
{
    public interface IApiClient<TEntity, TResource, TIdentifier>
    {
        public HttpResponseMessage Get();

        public HttpResponseMessage Get(TIdentifier identifier);

        public TEntity Read(HttpResponseMessage response);

        public TEntity Read(TIdentifier identifier);

        public IEnumerable<TEntity> ReadMany(HttpResponseMessage response);

        public HttpResponseMessage Post(TResource obj);

        public HttpResponseMessage Post(TEntity obj);

        public HttpResponseMessage Put(TResource obj);

        public HttpResponseMessage Put(TEntity obj);

        public HttpResponseMessage Delete(TIdentifier identifier);
    }
}
