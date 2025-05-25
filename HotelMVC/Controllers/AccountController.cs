using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HotelMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly HotelContext _context;

        public AccountController(HotelContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(User loginModel)
        {
            // ��������, ��� � ��� ���� � ����� � ������
            if (string.IsNullOrEmpty(loginModel.Username) || string.IsNullOrEmpty(loginModel.Password))
            {
                ModelState.AddModelError("", "������� ����� � ������");
                return View(loginModel);
            }

            // ���� ������������, ��������� AsNoTracking() ��� ������ ������������������ ��� ������
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == loginModel.Username);

            if (user != null && user.Password == loginModel.Password)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var claimsIdentity = new ClaimsIdentity(claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "�������� ����� ��� ������");
            return View(loginModel);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                // ���������, ���������� �� ��� ������������ � ����� ������
                var existingUser = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Username == user.Username);

                if (existingUser != null)
                {
                    ModelState.AddModelError("", "������������ � ����� ������ ��� ����������");
                    return View(user);
                }

                try
                {
                    // ������������� ���� �� ���������
                    user.Role = "User";

                    // ��������� ������������ � ��������
                    _context.Users.Add(user);

                    // ��������� ���������
                    await _context.SaveChangesAsync();

                    // ����� �������� ����������� ��������� ����
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Role, user.Role)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims,
                        CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "��������� ������ ��� �����������. ���������� �����.");
                    // ����� ����� ������������ ����������
                }
            }

            return View(user);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}