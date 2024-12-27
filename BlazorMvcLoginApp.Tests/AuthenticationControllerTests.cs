using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using BlazorMvcLoginApp.Controllers;
using BlazorMvcLoginApp.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;

public class AuthenticationControllerTests
{
    private readonly Mock<IAuthenticationService> _authenticationServiceMock;
    private readonly Mock<ICaptchaValidator> _captchaValidatorMock;
    private readonly AuthenticationController _controller;
    private readonly ServiceProvider _serviceProvider;

    public AuthenticationControllerTests()
    {
        _authenticationServiceMock = new Mock<IAuthenticationService>();
        _captchaValidatorMock = new Mock<ICaptchaValidator>();

        var services = new ServiceCollection();
        services.AddSingleton(_authenticationServiceMock.Object);
        services.AddSingleton<ITempDataDictionaryFactory, TempDataDictionaryFactory>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddLogging();  // Add logging services
        services.AddControllersWithViews();

        _serviceProvider = services.BuildServiceProvider();

        var httpContext = new DefaultHttpContext
        {
            RequestServices = _serviceProvider
        };

        _controller = new AuthenticationController(_captchaValidatorMock.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };
    }

    [Fact]
    public async Task Login_ValidCaptcha_ReturnsRedirectToHome()
    {
        // Arrange
        var user = new UserModel { UserName = "testuser" };
        _captchaValidatorMock.Setup(c => c.Validate(It.IsAny<string>())).Returns(true);

        // Mock Captcha validation
        _controller.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
        {
            { "CaptchaCode", "valid-captcha-code" }
        });

        // Act
        var result = await _controller.Login(user, "valid-captcha-code");

        // Assert
        var redirectResult = Assert.IsType<RedirectResult>(result);
        Assert.Equal("/Home", redirectResult.Url);
    }

    [Fact]
    public async Task Login_InvalidCaptcha_ReturnsView()
    {
        // Arrange
        var user = new UserModel { UserName = "testuser" };
        _captchaValidatorMock.Setup(c => c.Validate(It.IsAny<string>())).Returns(false);

        // Mock Captcha validation
        _controller.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
        {
            { "CaptchaCode", "invalid-captcha-code" }
        });

        // Act
        var result = await _controller.Login(user, "invalid-captcha-code");

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Logout_ReturnsRedirectToRoot()
    {
        // Act
        var result = await _controller.Logout();

        // Assert
        var redirectResult = Assert.IsType<RedirectResult>(result);
        Assert.Equal("/", redirectResult.Url);
    }
}
