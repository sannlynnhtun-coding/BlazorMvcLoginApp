namespace BlazorMvcLoginApp.Services;

public interface ICaptchaValidator
{
    bool Validate(string userInput);
}

