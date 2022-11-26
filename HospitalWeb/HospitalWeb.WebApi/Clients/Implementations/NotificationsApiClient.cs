using AutoMapper;
using HospitalWeb.DAL.Entities;
using HospitalWeb.WebApi.Clients.Interfaces;
using HospitalWeb.WebApi.Models.ResourceModels;

namespace HospitalWeb.WebApi.Clients.Implementations
{
    public class NotificationsApiClient : ApiClient<Notification, NotificationResourceModel, int>
    {
        public NotificationsApiClient(IConfiguration config) : base(config)
        {
        }

        public override HttpResponseMessage Get()
        {
            return _client.GetAsync("Notifications").Result;
        }

        public override HttpResponseMessage Get(int identifier)
        {
            return _client.GetAsync($"Notifications/{identifier}").Result;
        }

        public HttpResponseMessage Filter(string owner, int pageSize = 10, int pageNumber = 1)
        {
            return _client.GetAsync($"Notifications/details?owner={owner}&pageSize={pageSize}&pageNumber={pageNumber}").Result;
        }

        public override Notification Read(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<Notification>().Result;
        }

        public override Notification Read(int identifier)
        {
            var response = Get(identifier);
            return Read(response);
        }

        public override IEnumerable<Notification> ReadMany(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<IEnumerable<Notification>>().Result;
        }

        public override HttpResponseMessage Post(NotificationResourceModel obj)
        {
            return _client.PostAsJsonAsync("Notifications", obj).Result;
        }

        public override HttpResponseMessage Post(Notification obj)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Notification, NotificationResourceModel>());
            var mapper = new Mapper(config);

            var model = mapper.Map<Notification, NotificationResourceModel>(obj);

            return Post(model);
        }

        public override HttpResponseMessage Put(Notification obj)
        {
            return _client.PutAsJsonAsync("Notifications", obj).Result;
        }

        public override HttpResponseMessage Delete(int identifier)
        {
            return _client.DeleteAsync($"Notifications/{identifier}").Result;
        }
    }
}
