using AspNetCoreMVC.Models;
using AspNetCoreMVC.Models.RepositoryPatterns;
using AspNetCoreMVC.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AspNetCoreMVC.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly IEmployeeRepository empRepo;
        private readonly IWebHostEnvironment webHostEnvironment;

        public HomeController(ILogger<HomeController> logger, IEmployeeRepository empRepo, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            this.empRepo = empRepo;
            this.webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {

            var model = empRepo.GetAllEmployee();
            return View(model);
        }

        public ViewResult Details(int? id)
        {
            //throw new Exception("Error in Details View");

            Employee emp = empRepo.GetEmployee(id.Value);
            if (emp == null) {
                Response.StatusCode = 404;
                return View("../CustomError/EmployeeNotFound", id.Value);
            }
            DetailsViewModel homeDetailsViewModel = new DetailsViewModel()
            {
                Employee = emp,
                PageTitle = "Employee Details"
            };

            return View(homeDetailsViewModel);
        }

        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }



        [HttpPost]
        public IActionResult Create(CreateViewModel employee)
        {
            if (ModelState.IsValid && employee.Photos.Count > 0)
            {
                string uniqueFileName = "";
                if (employee.Photos != null)
                {
                    uniqueFileName = ProcessUploadedImages(employee);
                }
                Employee newEmployee = empRepo.Add(new Employee
                {
                    Name = employee.Name,
                    Department = employee.Department,
                    Email = employee.Email,
                    PhotoPath = uniqueFileName
                });
                return RedirectToAction("details", new { id = newEmployee.Id });
            }
            else
                return View();
        }

        [HttpGet]
        public ViewResult edit(int id)
        {
            Employee employee = empRepo.GetEmployee(id);
            EditViewModel editViewModel = new EditViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Department = employee.Department,
                Email = employee.Email,
                ExistingPhotoPath = employee.PhotoPath
            };
            return View(editViewModel);
        }

        [HttpPost]
        public IActionResult edit(EditViewModel employee)
        {
            if (ModelState.IsValid)
            {
                Employee emp = empRepo.GetEmployee(employee.Id);
                emp.Name = employee.Name;
                emp.Department = employee.Department;
                emp.Email = employee.Email;

                if (employee.Photos != null)
                {
                    emp.PhotoPath = ProcessUploadedImages(employee);
                    if (employee.ExistingPhotoPath != null)
                    {
                        string deleteImg = Path.Combine(webHostEnvironment.WebRootPath, "images", employee.ExistingPhotoPath);
                        System.IO.File.Delete(deleteImg);
                    }
                }
                else
                {
                    //This is not required as emp already contains the previous value
                    emp.PhotoPath = employee.ExistingPhotoPath;
                }
                empRepo.Update(emp);
                //return RedirectToAction("details", new { id = emp.Id });
                return RedirectToAction("index");
            }
            else
                return View();
        }

        private string ProcessUploadedImages(CreateViewModel employee)
        {
            string uniqueFileName = "";
            foreach (IFormFile photo in employee.Photos)
            {
                string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                string filepath = Path.Combine(uploadFolder, uniqueFileName);
                using (var fileStream = new FileStream(filepath, FileMode.Create))
                {
                    photo.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}