using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UniversityAcademicManagementSystem.Models;
using UniversityAcademicManagementSystem.Services.Interfaces;

namespace UniversityAcademicManagementSystem.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var email = User.Identity?.Name;
            var action = context.RouteData.Values["action"]?.ToString();

            if (User.IsInRole("Student") && email != null && action != "CompleteProfile" && action != "Logout")
            {
                var isComplete = _studentService.IsProfileCompleteAsync(email).Result;
                if (!isComplete)
                {
                    context.Result = new RedirectToActionResult("CompleteProfile", "Student", null);
                }
            }
            base.OnActionExecuting(context);
        }

        public async Task<IActionResult> Index()
        {
            var student = await _studentService.GetStudentByEmailAsync(User.Identity.Name);
            return View(student);
        }

        private List<string> GetDepartments()
        {
            return new List<string>
        {
            "Computer Science",
            "Information Technology",
            "Mechanical Engineering",
            "Electrical Engineering",
            "Civil Engineering"
        };
        }

        [HttpGet]
        public async Task<IActionResult> CompleteProfile()
        {
            var email = User.Identity?.Name;
            var student = await _studentService.GetStudentByEmailAsync(email);

            if (student == null) return NotFound();

            ViewBag.Departments = GetDepartments();

            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> CompleteProfile(Student model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = GetDepartments();
            }

            var student = await _studentService.GetStudentByEmailAsync(User.Identity.Name);
            if (student != null)
            {
                student.Name = model.Name;
                student.ContactNumber = model.ContactNumber;
                student.Department = model.Department;
                student.EnrollmentYear = model.EnrollmentYear;

                var result = await _studentService.UpdateStudentProfileAsync(student);
                if (result)
                {
                    return RedirectToAction("Index");
                }
            }
            ModelState.AddModelError("", "Something went wrong while saving data.");
            ViewBag.Departments = GetDepartments();
            return View(model);
        }
    }
}
