using AutoMapper;
using HospitalWeb.Domain.Entities;
using HospitalWeb.Domain.Services.Interfaces;
using HospitalWeb.WebApi.Models.ResourceModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalWeb.WebApi.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("api/[controller]")]
    public class GradesController : ControllerBase
    {
        private readonly ILogger<GradesController> _logger;
        private readonly IUnitOfWork _uow;

        public GradesController(
            ILogger<GradesController> logger,
            IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Returns a list of Grades
        /// </summary>
        /// <returns> A list of Grades </returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Grade>>> Get()
        {
            try
            {
                var grades = await _uow.Grades.GetAllAsync(include: g => g
                    .Include(g => g.Target)
                        .ThenInclude(d => d.Hospital)
                    .Include(g => g.Author));

                return new ObjectResult(grades);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in GradesController.Get(): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Returns the Grade object found by Id
        /// </summary>
        /// <param name="id"> Address id </param>
        /// <returns> The Grade object </returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Grade>> Get(int id)
        {
            try
            {
                var grade = await _uow.Grades.GetAsync(filter: g => g.GradeId == id, include: g => g
                    .Include(g => g.Target)
                        .ThenInclude(d => d.Hospital)
                    .Include(g => g.Author));

                if (grade == null)
                {
                    return NotFound("The grade object wasn't found");
                }

                return new ObjectResult(grade);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in GradesController.Get(id): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Returns Grades found by author and target
        /// </summary>
        /// <param name="author"> Grade author </param>
        /// <param name="target"> Grade target </param>
        /// <returns> The Address object </returns>
        [HttpGet("details")]
        public async Task<ActionResult<IEnumerable<Grade>>> Get(string author = null, string target = null)
        {
            try
            {
                Func<Grade, bool> filter = (g) =>
                {
                    bool result = true;

                    if (!string.IsNullOrWhiteSpace(author))
                    {
                        result = result && g.AuthorId == author;
                    }

                    if (!string.IsNullOrWhiteSpace(target))
                    {
                        result = result && g.TargetId == target;
                    }

                    return result;
                };

                var grades = await _uow.Grades.GetAllAsync(filter:filter, include: g => g
                    .Include(g => g.Target)
                        .ThenInclude(d => d.Hospital)
                    .Include(g => g.Author));

                return new ObjectResult(grades);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in GradesController.Get(author, target): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Creates a new Grade object
        /// </summary>
        /// <param name="grade"> The Grade object  </param>
        /// <returns> The Grade object </returns>
        [HttpPost]
        public async Task<ActionResult<Address>> Post(GradeResourceModel grade)
        {
            try
            {
                if (grade == null)
                {
                    return BadRequest("Passing null object to the GradesController.Post method");
                }

                var config = new MapperConfiguration(cfg => cfg.CreateMap<GradeResourceModel, Grade>());
                var mapper = new Mapper(config);

                var entity = mapper.Map<GradeResourceModel, Grade>(grade);

                await _uow.Grades.CreateAsync(entity);

                _logger.LogDebug($"Created grade with id {entity.GradeId}");

                return Ok(entity);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in GradesController.Post: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Updates the Grade object data
        /// </summary>
        /// <param name="grade"> The Grade object  </param>
        /// <returns> The Grade object </returns>
        [HttpPut]
        public async Task<ActionResult<Grade>> Put(Grade grade)
        {
            try
            {
                if (grade == null)
                {
                    return BadRequest("Passing null object to the GradesController.Put method");
                }

                await _uow.Grades.UpdateAsync(grade);

                _logger.LogDebug($"Updated grade with id {grade.GradeId}");

                return Ok(grade);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in GradesController.Put: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Deletes the Grade object
        /// </summary>
        /// <param name="id"> The Grade object id </param>
        /// <returns> The Grade object </returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Grade>> Delete(int id)
        {
            try
            {
                var grade = await _uow.Grades.GetAsync(g => g.GradeId == id);

                if (grade == null)
                {
                    return NotFound("The grade object wasn't found");
                }

                await _uow.Grades.DeleteAsync(grade);

                _logger.LogDebug($"Deleted grade with id {grade.GradeId}");

                return Ok(grade);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in GradesController.Delete: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }
    }
}
