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
                HttpResponseMessage response;

                if (stars == 0)
                {
                    response = _api.Grades.Filter(author, target);
                    if (response.IsSuccessStatusCode)
                    {
                        var grade = _api.Grades.ReadMany(response).FirstOrDefault();
                        if (grade != null)
                        {
                            _api.Grades.Delete(grade.GradeId);
                        }
                    }
                }
                else
                {
                    _api.Grades.AddOrUpdate(stars, author, target);
                }

                response = _api.Doctors.Get(target, null, null);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.StatusCode.ToString());
                }

                double rating = _api.Doctors.Read(response).Rating;

                await Clients.All.SendAsync("ChangeRating", rating, target);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }
    }
}
