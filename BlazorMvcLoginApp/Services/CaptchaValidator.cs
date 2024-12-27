using BotDetect.Web.Mvc;

namespace BlazorMvcLoginApp.Services;

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

