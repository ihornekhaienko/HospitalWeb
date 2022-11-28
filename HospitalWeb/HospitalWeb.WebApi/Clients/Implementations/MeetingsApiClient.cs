using AutoMapper;
using HospitalWeb.DAL.Entities;
using HospitalWeb.WebApi.Clients.Interfaces;
using HospitalWeb.WebApi.Models.ResourceModels;

namespace HospitalWeb.WebApi.Clients.Implementations
{
    public class MeetingsApiClient : ApiClient<Meeting, MeetingResourceModel, int>
    {
        public MeetingsApiClient(IConfiguration config) : base(config)
        {
        }

        public override HttpResponseMessage Get()
        {
            return _client.GetAsync("Meetings").Result;
        }

        public override HttpResponseMessage Get(int identifier)
        {
            return _client.GetAsync($"Meetings/{identifier}").Result;
        }

        public override Meeting Read(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<Meeting>().Result;
        }

        public override Meeting Read(int identifier)
        {
            var response = Get(identifier);
            return Read(response);
        }

        public override IEnumerable<Meeting> ReadMany(HttpResponseMessage response)
        {
            return response.Content.ReadAsAsync<IEnumerable<Meeting>>().Result;
        }

        public override HttpResponseMessage Post(MeetingResourceModel obj)
        {
            return _client.PostAsJsonAsync("Meetings", obj).Result;
        }

        public override HttpResponseMessage Post(Meeting obj)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Meeting, MeetingResourceModel>());
            var mapper = new Mapper(config);

            var model = mapper.Map<Meeting, MeetingResourceModel>(obj);

            return Post(model);
        }

        public override HttpResponseMessage Put(Meeting obj)
        {
            return _client.PutAsJsonAsync("Meetings", obj).Result;
        }

        public override HttpResponseMessage Delete(int identifier)
        {
            return _client.DeleteAsync($"Meetings/{identifier}").Result;
        }
    }
}
