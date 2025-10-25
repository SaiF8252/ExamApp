using ExamApp.Models;
using ExamApp.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace ExamApp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private MyExamEntities db = new MyExamEntities();

        // GET: Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel login, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var validUser = db.Users.FirstOrDefault(u => u.UserName == login.UserName && u.Password == login.Password);

                if (validUser != null)
                {
                    FormsAuthentication.SetAuthCookie(validUser.UserName, login.RememberMe);

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid username or password.");
            }

            return View(login);
        }

        // GET: Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        // POST: Register
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel register)
        {
            if (ModelState.IsValid)
            {
                if (db.Users.Any(u => u.UserName == register.UserName))
                {
                    ModelState.AddModelError("UserName", "Username already exists.");
                    return View(register);
                }

                User user = new User
                {
                    UserName = register.UserName,
                    Password = register.Password
                };

                db.Users.Add(user);
                db.SaveChanges();

                return RedirectToAction("Login");
            }

            return View(register);
        }

        // Logout
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
