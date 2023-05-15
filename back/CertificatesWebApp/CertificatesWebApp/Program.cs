using CertificatesWebApp.Certificates.Repositories;
using CertificatesWebApp.Exceptions;
using CertificatesWebApp.Security;
using CertificatesWebApp.Users.Repositories;
using CertificatesWebApp.Users.Services;
using Data.Context;
using Microsoft.AspNetCore.Authentication.Cookies;

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

//Security
builder.Services.AddTransient<CustomCookieAuthenticationEvents>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder =>
        {
            builder.WithOrigins("http://localhost:5173")
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
           options.ClientId = "749091886975-avudqlppv5vb78ic1jjeq30b4gjj73f0.apps.googleusercontent.com";
           options.ClientSecret = "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";
           options.CallbackPath = "/handle-signin-google";
           options.Scope.Add("https://www.googleapis.com/auth/userinfo.profile");
           options.Scope.Add("https://www.googleapis.com/auth/userinfo.email");
       });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReactApp");
app.UseMiddleware<ExceptionMiddleware>(true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
