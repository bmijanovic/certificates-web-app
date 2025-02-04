using CertificatesWebApp.Certificates.Repositories;
using CertificatesWebApp.Infrastructure;
using CertificatesWebApp.Security;
using CertificatesWebApp.Users.Repositories;
using CertificatesWebApp.Users.Services;
using Data.Context;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Runtime.ConstrainedExecution;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<CertificatesWebAppContext>();

//Repositories
builder.Services.AddTransient<IAdminRepository, AdminRepository>();
builder.Services.AddTransient<ICertificateRepository, CertificateRepository>();
builder.Services.AddTransient<ICertificateRequestRepository, CertificateRequestRepository>();
builder.Services.AddTransient<IConfirmationRepository, ConfirmationRepository>();
builder.Services.AddTransient<ICredentialsRepository, CredentialsRepository>();
builder.Services.AddTransient<IPasswordRecordRepository, PasswordRecordRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();


//Services
builder.Services.AddTransient<IAdminService, AdminService>();
builder.Services.AddTransient<ICertificateService, CertificateService>();
builder.Services.AddTransient<ICertificateRequestService, CertificateRequestService>();
builder.Services.AddTransient<IConfirmationService, ConfirmationService>();
builder.Services.AddTransient<ICredentialsService, CredentialsService>();
builder.Services.AddTransient<IMailService, MailService>();
builder.Services.AddTransient<ISMSService, SMSService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IGoogleCaptchaService, GoogleCaptchaService>();

//Security
builder.Services.AddTransient<CustomCookieAuthenticationEvents>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder =>
        {
            builder.WithOrigins("http://localhost:5173", "https://accounts.google.com")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
       .AddCookie(options =>
       {
           options.Cookie.SameSite = SameSiteMode.None;
           options.Cookie.Name = "auth";
           options.SlidingExpiration = true;
           options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
           options.Cookie.MaxAge = options.ExpireTimeSpan;
           options.EventsType = typeof(CustomCookieAuthenticationEvents);
       }).AddGoogle(options =>
       {
           StreamReader sr = new StreamReader("oauth_key.txt");
           String clientId = sr.ReadLine();
           String clientSecret = sr.ReadLine();
           options.ClientId = clientId;
           options.ClientSecret = clientSecret;
           options.CallbackPath = "/api/User/handle-signin-google";
           options.Scope.Add("https://www.googleapis.com/auth/userinfo.profile");
           options.Scope.Add("https://www.googleapis.com/auth/userinfo.email");
       });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AuthorizationPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("TwoFactor", "Confirmed");
        policy.RequireClaim("PasswordExpired", "False");
    });
});

builder.Services.AddSingleton<
    IAuthorizationMiddlewareResultHandler, AuthorizationMiddleware>();
/*builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureHttpsDefaults(listenOptions =>
    {
        var cert = "00C6F0066DC058B5EA";
        if (File.Exists($"Certs/{cert}.crt"))
        {
            var certificate = new X509Certificate2($"Certs/{cert}.crt");
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportRSAPrivateKey(File.ReadAllBytes($"Keys/{cert}.key"), out _);
                certificate = certificate.CopyWithPrivateKey(rsa);
            }
            listenOptions.ServerCertificate=certificate;
        }
        else
        {
            throw new NotImplementedException();
        }
    });

});*/

builder.Host.UseSerilog((context, configuration) => 
    configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseCors("AllowReactApp");
app.UseMiddleware<ExceptionMiddleware>(false);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
