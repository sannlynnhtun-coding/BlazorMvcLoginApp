using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BlazorMvcLoginApp.Models;
using BotDetect.Web.Mvc;

namespace BlazorMvcLoginApp.Controllers
{
    public class AuthenticationController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [CaptchaValidationActionFilter("CaptchaCode", "SystemCaptcha", "Wrong Captcha!")]
        public async Task<IActionResult> Login(UserModel user)
        {
            string userInput = HttpContext.Request.Form["CaptchaCode"]!;
            MvcCaptcha mvcCaptcha = new MvcCaptcha("SystemCaptcha");

            if (!mvcCaptcha.Validate(userInput))
            {
                MvcCaptcha.ResetCaptcha("ExampleCaptcha");
                return View();
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.Role, "User"),
                new(ClaimTypes.Role, "Admin"),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(principal);

            return Redirect("/Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }
}
