using CertificatesWebApp.Certificates.Repositories;
using CertificatesWebApp.Users.Repositories;
using CertificatesWebApp.Users.Services;
using Data.Context;
using Microsoft.EntityFrameworkCore;

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
builder.Services.AddTransient<IUserService, UserService>();

builder.Services.AddCors(feature =>
                feature.AddPolicy(
                    "CorsPolicy",
                    apiPolicy => apiPolicy
                                    //.AllowAnyOrigin()
                                    //.WithOrigins("http://localhost:4200")
                                    .AllowAnyHeader()
                                    .AllowAnyMethod()
                                    .SetIsOriginAllowed(host => true)
                                    .AllowCredentials()
                                ));

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
