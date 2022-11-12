using HospitalWeb.DAL.Services.Implementations;
using HospitalWeb.Filters.Builders.Implementations;
using HospitalWeb.Filters.Models.SortStates;
using HospitalWeb.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HospitalWeb.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly ILogger<DoctorsController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly UnitOfWork _uow;
        private readonly IFileManager _fileManager;

        public DoctorsController(
            ILogger<DoctorsController> logger,
            IWebHostEnvironment environment,
            UnitOfWork uow,
            IFileManager fileManager
            )
        {
            _logger = logger;
            _environment = environment;
            _uow = uow;
            _fileManager = fileManager;
        }

        [HttpGet]
        public async Task<IActionResult> Search(
            string searchString,
            int? specialty,
            int page = 1,
            DoctorSortState sortOrder = DoctorSortState.Id)
        {
            ViewBag.Image = await _fileManager.GetBytes(Path.Combine(_environment.WebRootPath, "files/images/profile.jpg"));

            var builder = new DoctorsViewModelBuilder(_uow, page, searchString, sortOrder, specialty);
            var director = new ViewModelBuilderDirector();
            director.MakeViewModel(builder);
            var viewModel = builder.GetViewModel();

            return View(viewModel);
        }
    }
}