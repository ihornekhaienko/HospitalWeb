namespace HospitalWeb.Mvc.Clients.Interfaces
{
    public interface IApiClient<TEntity, TResource, TIdentifier>
    {
        HttpResponseMessage Get(string token = null, string provider = null);

        HttpResponseMessage Get(TIdentifier identifier, string token = null, string provider = null);

        TEntity Read(HttpResponseMessage response);

        TEntity Read(TIdentifier identifier);

        IEnumerable<TEntity> ReadMany(HttpResponseMessage response);

        TErrorResponse ReadError<TErrorResponse>(HttpResponseMessage response);

        HttpResponseMessage Post(TResource obj, string token = null, string provider = null);

        HttpResponseMessage Post(TEntity obj, string token = null, string provider = null);

        HttpResponseMessage Put(TEntity obj, string token = null, string provider = null);

        HttpResponseMessage Delete(TIdentifier identifier, string token = null, string provider = null);
    }
}
