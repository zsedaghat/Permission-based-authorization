using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SpaceManagment;
using SpaceManagment.Common;
using SpaceManagment.Configuration;
using SpaceManagment.Data;
using SpaceManagment.Model;
using SpaceManagment.Service;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();
// Add services to the container.
Settings settings = config.GetRequiredSection("Settings").Get<Settings>();

builder.Services.Configure<Settings>(builder.Configuration.GetSection("Settings"));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//ServiceCollectionExtensions
builder.AddIdentity(settings.IdentitySettings);
builder.AddDbContext(settings.ConnectionStrings);
builder.AddSwagger();
builder.AddAuthenticationJwt(settings.JwtSettings);
//dependencyInjection

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IJwtService, JwtService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
