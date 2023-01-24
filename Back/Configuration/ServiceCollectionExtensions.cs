using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SpaceManagment.Common;
using SpaceManagment.Data;
using SpaceManagment.Model;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

namespace SpaceManagment.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSwagger(this WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                options.OperationFilter<SecurityRequirementsOperationFilter>();
            });
        }

        public static void AddAuthenticationJwt(this WebApplicationBuilder builder, JwtSettings settings)
        {
            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                var Key = Encoding.UTF8.GetBytes(settings.Key);
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = settings.Issuer,
                    ValidAudience = settings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Key)
                };
            });
        }

        public static void AddIdentity(this WebApplicationBuilder builder, IdentitySettings settings)
        {
            builder.Services.AddIdentity<User, Role>(identityOptions =>
            {
                //Password Settings
                identityOptions.Password.RequireDigit = settings.PasswordRequireDigit;
                identityOptions.Password.RequiredLength = settings.PasswordRequiredLength;
                identityOptions.Password.RequireNonAlphanumeric = settings.PasswordRequireNonAlphanumic; //#@!
                identityOptions.Password.RequireUppercase = settings.PasswordRequireUppercase;
                identityOptions.Password.RequireLowercase = settings.PasswordRequireLowercase;

                //UserName Settings
                //identityOptions.User.RequireUniqueEmail = settings.RequireUniqueEmail;
            })
    .AddEntityFrameworkStores<SpaceManagmentDbContext>()
    .AddDefaultTokenProviders();
        }

        public static void AddDbContext(this WebApplicationBuilder builder, ConnectionStrings settings)
        {

            builder.Services.AddDbContext<SpaceManagmentDbContext>(options =>
            {
                options
                    .UseSqlServer(settings.SqlServer);
                //Tips
                // .ConfigureWarnings(warning => warning.Throw(RelationalEventId.QueryClientEvaluationWarning));
            });
        }
    }
}
