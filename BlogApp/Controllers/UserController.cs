using BlogApp.Data.Abstract;
using BlogApp.Data.Concrete;
using BlogApp.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers
{
    public class UserController : Controller
    {
        readonly IUserRepository _UserRepository;
        public UserController(IUserRepository UserRepository)
        {
            _UserRepository = UserRepository;
        }
        public IActionResult Logın()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Post");
            }

            return View();
        }
        public IActionResult Register()
        { 
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _UserRepository.Users.FirstOrDefaultAsync(x => x.UserName == model.UserName || x.Email == model.Email);
                if (user == null)
                {
                    _UserRepository.CreateUser(new Entity.User
                    {
                        UserName = model.UserName,
                        Name = model.Name,
                        Email = model.Email,
                        Password = model.Password,
                        Image = "avatar.jpg"

                    });
                    return RedirectToAction("Logın");
                }
                else
                {
                    ModelState.AddModelError("", "Username ya da Email kullanımda.");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Logın");
        }
        
        [HttpPost]
        public async Task<IActionResult> Logın(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var isUser = _UserRepository.Users.FirstOrDefault(x => x.Email == model.Email && x.Password == model.Password);

                if (isUser != null)
                {
                    var userClaims = new List<Claim>();

                    userClaims.Add(new Claim(ClaimTypes.NameIdentifier, isUser.UserId.ToString()));
                    userClaims.Add(new Claim(ClaimTypes.Name, isUser.UserName ?? ""));
                    userClaims.Add(new Claim(ClaimTypes.GivenName, isUser.Name ?? ""));
                    userClaims.Add(new Claim(ClaimTypes.UserData, isUser.Image ?? ""));

                    if (isUser.Email == "info@serdalozsoy.com")
                    {
                        userClaims.Add(new Claim(ClaimTypes.Role, "admin"));
                    }

                    var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true
                    };

                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToAction("Index", "Post");
                }
                else
                {
                    ModelState.AddModelError("", "Kullanıcı adı veya şifre yanlış");
                }

            }

            return View(model);
        }
        public IActionResult Profile(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return NotFound();
            }
            var user = _UserRepository
                        .Users
                        .Include(x => x.Posts)
                        .Include(x => x.Comments)
                        .ThenInclude(x => x.Post)
                        .FirstOrDefault(x => x.UserName == username);

            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

    }

}
