
using FluentValidation;
using Messenger.Api.Areas.Cryptography.Abstractions;
using Messenger.Api.Areas.Cryptography.Services;
using Messenger.Api.Areas.Data.Abstractions;
using Messenger.Api.Areas.Data.Services;
using Messenger.Api.Areas.Validation;
using Messenger.Api.Utilities;
using Messenger.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Messenger.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
            builder.Services.AddScoped<IPasswordChecker, PasswordChecker>();
            builder.Services.AddScoped<IJsonWebTokenProvider, JsonWebTokenProvider>();
            builder.Services.AddScoped<IUserReader, UserReader>();
            builder.Services.AddScoped<ISocialConnectionsReader, SocialConnectionsReader>();
            builder.Services.AddScoped<IUserWriter, UserWriter>();
            builder.Services.AddScoped<IValidator<RegisterDto>, RegisterDtoValidator>();
            builder.Services.AddScoped<IValidator<UsernameLoginDto>, UsernameLoginDtoValidator>();
            builder.Services.AddScoped<RandomUserCodeGenerator>();

            // Json Web Token Authentication / Authorization
            builder.Services.AddAuthorization();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JsonWebToken:SecretKey"]!)),
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["JsonWebToken:Issuer"],
                        ValidAudience = builder.Configuration["JsonWebToken:Audience"],
                        ClockSkew = TimeSpan.Zero,
                        NameClaimType = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub,
                    };
                });

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
