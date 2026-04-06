using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityAcademicManagementSystem.Models;
using UniversityAcademicManagementSystem.Services.Interfaces;

namespace UniversityAcademicManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAdminService _adminService;

        public AdminController(IUserService userService, IAdminService adminService)
        {
            _userService = userService;
            _adminService = adminService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _adminService.GetAllStaffAsync();
            return View(users);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(User model)
        {
            var result = await _adminService.RegisterStaffAsync(model);

            if (result)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("Email", "This email is already registered.");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _adminService.DeleteStaffAsync(id);

            if (result)
            {
                TempData["SuccessMessage"] = "Staff member removed successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Error occurred while removing staff.";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(User model)
        {
            ModelState.Remove("ConfirmPassword");

            if (ModelState.IsValid)
            {
                var result = await _adminService.UpdateStaffAsync(model);
                if (result)
                {
                    TempData["SuccessMessage"] = "Staff updated successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Email", "This email is already registered with another account.");
                }
            }
            return View(model);
        }
    }
}
