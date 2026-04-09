using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityAcademicManagementSystem.Models;
using UniversityAcademicManagementSystem.Services.Interfaces;

namespace UniversityAcademicManagementSystem.Controllers
{
    [Authorize(Roles = "Faculty")]
    public class FacultyController : Controller
    {
        private readonly IFacultyService _facultyService;
        private List<string> Departments = new List<string> { "Computer Science", "Information Technology", "Mechanical Engineering", "Electrical Engineering", "Civil Engineering" };

        public FacultyController(IFacultyService facultyService) => _facultyService = facultyService;

        public async Task<IActionResult> Index()
        {
            var facultyDept = GetFacultyDepartment();
            return View(await _facultyService.GetGradesByDepartmentAsync(facultyDept));
        }

        [HttpGet]
        public IActionResult AddGrade()
        {
            var facultyDept = GetFacultyDepartment();
            ViewBag.Departments = new List<string> { facultyDept! }; // ✅ Only own dept
            return View(new Grade());
        }

        [HttpPost]
        public async Task<IActionResult> AddGrade(Grade model, string dept, string actionType)
        {
            var facultyDept = GetFacultyDepartment();

            // 🚫 Department mismatch (UI + security)
            if (!string.IsNullOrEmpty(dept) && dept != facultyDept)
            {
                ModelState.AddModelError("", "You don't belong to this department.");
                ViewBag.Departments = new List<string> { facultyDept! };
                return View(model);
            }

            ViewBag.Departments = new List<string> { facultyDept! };
            ViewBag.SelectedDept = dept;
            ViewBag.SelectedCourse = model.CourseId;

            if (actionType == "Refresh")
            {
                if (!string.IsNullOrEmpty(dept))
                    ViewBag.Courses = await _facultyService.GetCoursesByDepartmentAsync(dept);

                if (model.CourseId > 0)
                    ViewBag.Students = await _facultyService.GetEnrolledStudentsByCourseAsync(model.CourseId);

                ModelState.Clear();
                return View(model);
            }

            // DUPLICATE CHECK
            if (await _facultyService.IsGradeAlreadyExists(model.StudentId, model.CourseId))
            {
                ModelState.AddModelError("", "Grade already exists for this student in this course.");
            }

            if (ModelState.IsValid)
            {
                if (await _facultyService.AddGradeAsync(model, facultyDept))
                {
                    TempData["Success"] = "Grade Saved Successfully!";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Unauthorized department access.");
            }

            if (!string.IsNullOrEmpty(dept))
                ViewBag.Courses = await _facultyService.GetCoursesByDepartmentAsync(dept);

            if (model.CourseId > 0)
                ViewBag.Students = await _facultyService.GetEnrolledStudentsByCourseAsync(model.CourseId);

            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var grade = await _facultyService.GetGradeByIdAsync(id);
            return View(grade);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Grade model)
        {
            if (await _facultyService.UpdateGradeAsync(model))
            {
                TempData["Success"] = "Grade Updated!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (await _facultyService.DeleteGradeAsync(id)) TempData["Success"] = "Grade Deleted!";
            else TempData["Error"] = "Delete Failed!";
            return RedirectToAction(nameof(Index));
        }

        private string? GetFacultyDepartment()
        {
            return User.Claims.FirstOrDefault(c => c.Type == "Department")?.Value;
        }
    }
}
