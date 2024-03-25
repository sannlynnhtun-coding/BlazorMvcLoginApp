https://captcha.com/captcha-examples.html

https://captcha.com/doc/aspnet/net-captcha-options.html

```csharp
PM> Install-Package Captcha
```

ASP.NET Core : Synchronous operations are disallowed. Call WriteAsync or set AllowSynchronousIO to true instead
```csharp
public void ConfigureServices(IServiceCollection services)
{
    // If using Kestrel:
    services.Configure<KestrelServerOptions>(options =>
    {
        options.AllowSynchronousIO = true;
    });

    // If using IIS:
    services.Configure<IISServerOptions>(options =>
    {
        options.AllowSynchronousIO = true;
    });
}
```