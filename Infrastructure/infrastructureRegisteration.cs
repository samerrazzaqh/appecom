using System.Text;
using Core.Entities;
using Core.interfaces;
using Core.Services;
using Infrastructure.Data;
using Infrastructure.Repositries;
using Infrastructure.Repositries.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace Infrastructure;

public static class infrastructureRegisteration
{
    public static IServiceCollection infrastructureConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        //apply DbContext
        services.AddDbContext<AppDbContext>(op =>
        {
            op.UseSqlServer(configuration.GetConnectionString("EcomDatabase"));
            //op.UseSqlite(configuration.GetConnectionString("EcomDatabase"));
            
        });
        services.AddScoped(typeof(IGenericRepositry<>), typeof(GenericRepositry<>));
        //apply Unit OF Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        //register email sender
        services.AddScoped<IEmailService, EmailService>();

        //register IOrder Service
        services.AddScoped<IOrderService, OrderService>();

        services.AddScoped<IRating, RatingRepositry>();
        //register token
        services.AddScoped<IGenerateToken, GenerateToken>();

        //register payment service
        services.AddScoped<IPaymentService, PaymentService>();


        //apply Redis Connectoon
        services.AddSingleton<IConnectionMultiplexer>(i =>
        {
            var config = ConfigurationOptions.Parse(configuration.GetConnectionString("redis"));
            config.AbortOnConnectFail = false;
            return ConnectionMultiplexer.Connect(config);
        });

        services.AddSingleton<IImageManagementService, ImageManagementService>();
        services.AddSingleton<IFileProvider>(
            new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));


        services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
        
        services.Configure<SecurityStampValidatorOptions>(options =>
        {
            options.ValidationInterval = TimeSpan.FromMinutes(1);
        });
        
        // ====================For Cookies===============================================================
        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie(x =>
        {
            x.Cookie.Name = "token";
            x.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };
        })
        .AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,

                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"])),
                ValidateIssuer = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };
            x.Events = new JwtBearerEvents
            {

                OnMessageReceived = context =>
                {
                    var token = context.Request.Cookies["token"];
                    context.Token = token;
                    return Task.CompletedTask;
                }
            };
        });


        // ====================For localStorage===============================================================
        
        // var secret = configuration.GetValue<string>("Jwt:Secret");
        // services.AddAuthentication(x =>
        //     {
        //         x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //         x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //     })
        //     .AddJwtBearer(x =>
        //     {
        //         x.TokenValidationParameters = new TokenValidationParameters
        //         {
        //             ValidateIssuer = true,
        //             ValidateAudience = true,
        //             ValidateLifetime = true,
        //             ValidateIssuerSigningKey = true,
        //             ValidIssuer = "doseHieu",
        //             ValidAudience = "doseHieu",
        //             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
        //         };
        //     });



        return services;
    }
}
