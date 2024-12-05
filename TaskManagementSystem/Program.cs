using System.Text;
using System.Text.Unicode;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TaskManagementSystem.BLL;
using TaskManagementSystem.BLL.Services;
using TaskManagementSystem.BLL.Validators;
using TaskManagementSystem.DAL;
using TaskManagementSystem.DAL.Entities;
using TaskManagementSystem.DAL.Repositories;
using TaskManagementSystem.DTO;

namespace TaskManagementSystem;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAutoMapper(typeof(MappingProfile));
        
        builder.Services.AddDbContext<TaskDbContext>(optionsBuilder =>
            optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddScoped<IValidator<LoginDto>, LoginDtoValidator>();
        builder.Services.AddScoped<IValidator<RegisterDto>, RegisterDtoValidator>();
        builder.Services.AddScoped<IValidator<UserTaskDto>, UserTaskDtoValidator>();
        
        builder.Services.AddSingleton<JwtGenerator>();

        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ITaskRepository, TaskRepository>();
        
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ITaskService, TaskService>();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(x =>
        {
            x.TokenValidationParameters = new TokenValidationParameters()
            {
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JwtConfiguration:SecretKey"])),
                ValidIssuer = builder.Configuration["JwtConfiguration:Issuer"],
                ValidAudience = builder.Configuration["JwtConfiguration:Audience"]
            };
            x.MapInboundClaims = false;
        });
        builder.Services.AddAuthorization();
        
        builder.Services.AddControllers();
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please insert JWT with Bearer into field",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });

        var app = builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();
        app.UseAuthorization();

        app.MapControllers();
        
        app.Run();
    }
}