using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BanKeKhaiNhapHoc.Data;
using BanKeKhaiNhapHoc.Models;
using System.Linq;

namespace BanKeKhaiNhapHoc.Controllers
{
    public class AccountController : Controller
    {
        private readonly BanKeKhaiNhapHocContext _context;

        public AccountController(BanKeKhaiNhapHocContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                HttpContext.Session.SetString("Username", user.Username);
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng!";
            return View();
        }

        // GET: /Account/Logout
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
