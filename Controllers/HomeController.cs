using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UniversityAcademicManagementSystem.Data;
using UniversityAcademicManagementSystem.Models;
using UniversityAcademicManagementSystem.Services.Implementations;
using UniversityAcademicManagementSystem.Services.Interfaces;

namespace UniversityAcademicManagementSystem.Controllers
{
    public class HomeController : Controller
    {

        private readonly IUserService _userService;
        private readonly IStudentService _studentService;
        private readonly IRegistrarService _registrarService;

        public HomeController(IUserService userService, IStudentService studentService, IRegistrarService registrarService)
        {
            _userService = userService;
            _studentService = studentService;
            _registrarService = registrarService;
        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin")) return RedirectToAction("Index", "Admin");
                if (User.IsInRole("Student")) return RedirectToAction("Index", "Student");
                if (User.IsInRole("Registrar")) return RedirectToAction("Index", "Registrar");
                if (User.IsInRole("Faculty")) return RedirectToAction("Index", "Faculty");
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _userService.RegisterUserAsync(model);

            if (result)
            {
                TempData["Success"] = "Registration Successful! Please Login.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("Email", "This email is already registered.");
            return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Email and Password are required.");
                return View();
            }

            var user = await _userService.LoginUserAsync(email, password);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim("UserId", user.UserId.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

                if (user.Role == Role.Admin)
                {
                    return RedirectToAction("Index", "Admin");
                }

                if(user.Role == Role.Faculty)
                {
                    return RedirectToAction("Index", "Faculty");
                }

                if(user.Role == Role.Registrar)
                {
                    return RedirectToAction("Index", "Registrar");
                }

                if(user.Role == Role.Student)
                {
                    var isProfileComplete = await _studentService.IsProfileCompleteAsync(user.Email);

                    if (!isProfileComplete)
                    {
                        TempData["Info"] = "Please complete your profile to continue.";
                        return RedirectToAction("CompleteProfile", "Student");
                    }

                    return RedirectToAction("Index", "Student");
                }

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid Email or Password.");
            return View();
        }

        [HttpPost]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            TempData.Clear();

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Error(int? statusCode = null)
        {
            return View();
        }

        public async Task<IActionResult> Courses()
        {
            var courses = await _registrarService.GetAllCoursesAsync();

            return View(courses);
        }
    }
}
