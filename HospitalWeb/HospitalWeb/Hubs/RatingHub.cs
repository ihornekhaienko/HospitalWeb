using HospitalWeb.Clients.Implementations;
using Microsoft.AspNetCore.SignalR;

namespace HospitalWeb.Hubs
{
    public class RatingHub : Hub
    {
        private readonly ApiUnitOfWork _api;

        public RatingHub(ApiUnitOfWork api)
        {
            _api = api;
        }

        public async Task ChangeRating(int stars, string author, string target)
        {
            try
            {
                _api.Grades.AddOrUpdate(stars, author, target);

                var response = _api.Doctors.Get(target, null, null);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.StatusCode.ToString());
                }

                double rating = _api.Doctors.Read(response).Rating;

                await Clients.All.SendAsync("ChangeRating", rating);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }
    }
}
