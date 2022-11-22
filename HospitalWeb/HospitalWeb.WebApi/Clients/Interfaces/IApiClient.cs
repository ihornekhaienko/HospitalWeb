namespace HospitalWeb.WebApi.Clients.Interfaces
{
    public interface IApiClient<TEntity, TResource, TIdentifier>
    {
        HttpResponseMessage Get();

        HttpResponseMessage Get(TIdentifier identifier);

        TEntity Read(HttpResponseMessage response);

        TEntity Read(TIdentifier identifier);

        IEnumerable<TEntity> ReadMany(HttpResponseMessage response);

        HttpResponseMessage Post(TResource obj);

        HttpResponseMessage Post(TEntity obj);

        HttpResponseMessage Put(TEntity obj);

        HttpResponseMessage Delete(TIdentifier identifier);
    }
}
