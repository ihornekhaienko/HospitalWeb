using HospitalWeb.Mvc.Filters.Builders.Implementations;
using HospitalWeb.Mvc.Services.Interfaces;
using HospitalWeb.Mvc.ViewModels.Error;
using HospitalWeb.Mvc.Clients.Implementations;
using HospitalWeb.Mvc.Models.SortStates;
using Microsoft.AspNetCore.Mvc;

namespace HospitalWeb.Mvc.Controllers
{
    public class HospitalsController : Controller
    {
        private readonly ILogger<HospitalsController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly ApiUnitOfWork _api;
        private readonly IFileManager _fileManager;

        public HospitalsController(
            ILogger<HospitalsController> logger, 
            IWebHostEnvironment environment, 
            ApiUnitOfWork api, 
            IFileManager fileManager)
        {
            _logger = logger;
            _environment = environment;
            _api = api;
            _fileManager = fileManager;
        }

        [HttpGet]
        public async Task<IActionResult> Search(
            string searchString,
            int? locality,
            int page = 1,
            HospitalSortState sortOrder = HospitalSortState.Id)
        {
            try
            {
                ViewBag.Image = await _fileManager.GetBytes(Path.Combine(_environment.WebRootPath, "files/images/hospital-icon.png"));

                var builder = new HospitalsViewModelBuilder(_api, page, searchString, sortOrder, locality);
                var director = new ViewModelBuilderDirector();
                director.MakeViewModel(builder);
                var viewModel = builder.GetViewModel();

                return View(viewModel);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in HospitalsController.Search.Get: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
        }
    }
}
