using HospitalWeb.WebApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HospitalWeb.WebApi.Controllers
{
    [ApiController]
    [Route("/")]
    public class TestController : ControllerBase
    {
        private readonly IDbInitializer _dbInitializer;

        public TestController(IDbInitializer dbInitializer)
        {
            _dbInitializer = dbInitializer;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await _dbInitializer.GenerateDb();

            return Ok();
        }
    }
}
