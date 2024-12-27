using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BlazorMvcLoginApp.Models;
using BotDetect.Web.Mvc;

namespace BlazorMvcLoginApp.Controllers;

public interface ICaptchaValidator
{
    bool Validate(string userInput);
}

public class CaptchaValidator : ICaptchaValidator
{
    private readonly string _captchaId;

    public CaptchaValidator(string captchaId)
    {
        _captchaId = captchaId;
    }

    public bool Validate(string userInput)
    {
        MvcCaptcha mvcCaptcha = new MvcCaptcha(_captchaId);
        return mvcCaptcha.Validate(userInput);
    }
}

public class AuthenticationController : Controller
{
    private readonly ICaptchaValidator _captchaValidator;

    public AuthenticationController(ICaptchaValidator captchaValidator)
    {
        _captchaValidator = captchaValidator;
    }

    public ActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(UserModel user, [FromForm] string captchaCode)
    {
        if (!_captchaValidator.Validate(captchaCode))
        {
            MvcCaptcha.ResetCaptcha("SystemCaptcha");
            return View();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, "User"),
            new Claim(ClaimTypes.Role, "Admin"),
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

