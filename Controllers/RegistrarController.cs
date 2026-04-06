using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityAcademicManagementSystem.Models;
using UniversityAcademicManagementSystem.Services.Interfaces;

namespace UniversityAcademicManagementSystem.Controllers
{
    [Authorize(Roles = "Registrar")]
    public class RegistrarController : Controller
    {
        private readonly IRegistrarService _registrarService;

        private readonly List<string> _departments = new()
        {
            "Computer Science", "Information Technology", "Mechanical Engineering",
            "Electrical Engineering", "Civil Engineering"
        };

        public RegistrarController(IRegistrarService registrarService)
        {
            _registrarService = registrarService;
        }

        public async Task<IActionResult> Index(string dept, string sem)
        {
            var courses = await _registrarService.GetAllCoursesAsync();

            if (!string.IsNullOrEmpty(dept))
            {
                courses = courses.Where(c => c.Department == dept);
            }

            if (!string.IsNullOrEmpty(sem))
            {
                courses = courses.Where(c => c.SemesterOffered == sem);
            }

            ViewBag.Departments = _departments;
            ViewBag.SelectedDept = dept;
            ViewBag.SelectedSem = sem;

            return View(courses);
        }

        public IActionResult Create()
        {
            ViewBag.Departments = _departments;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Course model)
        {
            if (ModelState.IsValid)
            {
                var result = await _registrarService.AddCourseAsync(model);

                if (result)
                {
                    TempData["SuccessMessage"] = "Course added successfully!";
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("", "This course already exists in this department for the selected semester.");
            }

            ViewBag.Departments = _departments;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _registrarService.GetCourseByIdAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            ViewBag.Departments = _departments;
            return View(course);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Course model)
        {
            if (ModelState.IsValid)
            {
                var result = await _registrarService.UpdateCourseAsync(model);

                if (result)
                {
                    TempData["SuccessMessage"] = "Course updated successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Update failed. Another course with the same details already exists.");
                }
            }

            ViewBag.Departments = _departments;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _registrarService.DeleteCourseAsync(id);

            if (result)
            {
                TempData["SuccessMessage"] = "Course removed successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Error occurred while removing the course.";
            }

            return RedirectToAction("Index");
        }
    }
}