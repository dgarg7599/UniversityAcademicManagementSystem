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

        public async Task<IActionResult> Profile()
        {
            var student = await _studentService.GetStudentByEmailAsync(User.Identity.Name);
            return View(student);
        }

        public async Task<IActionResult> AvailableCourses()
        {
            var student = await _studentService.GetStudentByEmailAsync(User.Identity.Name);
            var courses = await _studentService.GetAvailableCoursesAsync(student.Department, student.StudentId);
            return View(courses);
        }

        public async Task<IActionResult> EnrolledCourses()
        {
            var student = await _studentService.GetStudentByEmailAsync(User.Identity.Name);
            var enrollments = await _studentService.GetStudentEnrollmentsAsync(student.StudentId);

            return View(enrollments);
        }

        [HttpPost]
        public async Task<IActionResult> Enroll(int courseId)
        {
            var student = await _studentService.GetStudentByEmailAsync(User.Identity.Name);

            var success = await _studentService.EnrollInCourseAsync(student.StudentId, courseId);

            if (success)
                TempData["Success"] = "Successfully enrolled in the course!";
            else
                TempData["Error"] = "Enrollment failed. You might already be enrolled.";

            return RedirectToAction(nameof(EnrolledCourses));
        }

        [HttpPost]
        public async Task<IActionResult> DropCourse(int enrollmentId)
        {
            var success = await _studentService.DropCourseAsync(enrollmentId);

            if (success)
                TempData["Success"] = "Course has been dropped.";
            else
                TempData["Error"] = "Unable to drop the course.";

            return RedirectToAction(nameof(EnrolledCourses));
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var student = await _studentService.GetStudentByEmailAsync(User.Identity.Name);
            if (student == null) return NotFound();

            ViewBag.Departments = GetDepartments();
            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(Student model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = GetDepartments();
                return View(model);
            }
            var student = await _studentService.GetStudentByEmailAsync(User.Identity.Name);

            if (student != null)
            {
                student.Name = model.Name;
                student.ContactNumber = model.ContactNumber;
                student.Department = student.Department;
                student.EnrollmentYear = model.EnrollmentYear;

                var result = await _studentService.UpdateStudentProfileAsync(student);
                if (result)
                {
                    TempData["Success"] = "Profile updated successfully!";
                    return RedirectToAction(nameof(Profile));
                }
            }
            ModelState.AddModelError("", "Unable to save changes. Try again.");
            ViewBag.Departments = GetDepartments();
            return View(model);
        }

        public async Task<IActionResult> Academics()
        {
            string userEmail = User.Identity.Name;

            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Home");
            }

            var student = await _studentService.GetStudentByEmailAsync(userEmail);

            if (student.StudentId == 0)
            {
                return NotFound("Student profile record not found for this email.");
            }

            var grades = await _studentService.GetStudentGradesAsync(student.StudentId);
            var (cgpa, totalCourses) = await _studentService.CalculateCGPAAsync(student.StudentId);

            ViewBag.CGPA = cgpa;
            ViewBag.TotalCourses = totalCourses;

            return View(grades);
        }
    }
}
