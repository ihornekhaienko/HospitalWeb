using HospitalWeb.Domain.Entities;
using HospitalWeb.Mvc.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace HospitalWeb.Mvc.Services.Implementations
{
    public class ZoomMeetingService : IMeetingService
    {
        private readonly IConfiguration _config;
        private readonly ILogger _logger;

        public ZoomMeetingService(IConfiguration config, ILogger<ZoomMeetingService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public Meeting CreateMeeting(Appointment appointment)
        {
            try
            {
                var meeting = new Meeting
                {
                    Topic = $"Appointment on {appointment.AppointmentDate.Date}",
                    Duration = 60,
                    AppointmentId = appointment.AppointmentId
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var apiSecret = _config["Zoom:ApiSecret"];
                var key = Encoding.UTF8.GetBytes(apiSecret);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Issuer = _config["Zoom:ApiKey"],
                    Expires = DateTime.Now.AddMinutes(5),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                var client = new RestClient($"https://api.zoom.us/v2/users/{_config["Zoom:Host"]}/");
                var request = new RestRequest("meetings", Method.Post);
                request.RequestFormat = DataFormat.Json;

                //type == 2 - A scheduled meeting.
                request.AddJsonBody(new 
                { 
                    topic = meeting.Topic, 
                    duration = meeting.Duration, 
                    start_time = appointment.AppointmentDate, 
                    type = "2",
                    waiting_room = true
                });
                request.AddHeader("authorization", $"Bearer {tokenString}");

                var response = client.Execute(request);
                var jObject = JObject.Parse(response.Content);

                meeting.StartLink = (string)jObject["start_url"];
                meeting.JoinLink = (string)jObject["join_url"];

                return meeting;
            }
            catch (Exception err)
            {
                _logger.LogCritical(err.Message);
                throw;
            }
        }
    }
}
