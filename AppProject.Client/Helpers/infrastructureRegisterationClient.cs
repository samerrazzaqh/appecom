using AppProject.Client.Authentication;
using AppProject.Client.Service.Account;
using AppProject.Client.Service.Administration;
using AppProject.Client.Service.Basket;
using AppProject.Client.Service.Categories;
using AppProject.Client.Service.Orders;
using AppProject.Client.Service.Payment;
using AppProject.Client.Service.Products;
using AppProject.Client.Service.Profile;
using AppProject.Client.Service.Rating;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using MudBlazor.Services;

namespace AppProject.Client.Helpers;

public static class infrastructureRegisterationClient
{
    public static IServiceCollection infrastructureConfigurationClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthorizationCore(options =>
            {
                options.AddPolicy("CanCreateRole", policy =>
                    policy.RequireClaim("Create Role", "true"));
                options.AddPolicy("CanEditRole", policy =>
                    policy.RequireClaim("Edit Role", "true"));
                options.AddPolicy("CanDeleteRole", policy =>
                    policy.RequireClaim("Delete Role", "true"));
            }
        );
        services.AddCascadingAuthenticationState();
        services.AddMudServices();
        services.AddBlazoredLocalStorage();

        services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
        services.AddScoped<CheckAndRefreshToken>();

        services.AddTransient<CookieHandler>();
        services.AddTransient<UnauthorizedHandler>();

        // 1. قراءة رابط الـ API من appsettings.json بدلاً من AppConstant
        var apiBaseAddress = configuration["ApiSettings:BaseApiAddress"] ?? "http://localhost:5001/";

        services.AddHttpClient("ServerApi", client => { client.BaseAddress = new Uri(apiBaseAddress); })
            .AddHttpMessageHandler<CookieHandler>()
            .AddHttpMessageHandler<UnauthorizedHandler>();

        services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServerApi"));

        // 2. تسجيل الخدمات باستعمال configuration إذا كانت AddApiService تتطلب ذلك
        services.AddApiService<IAccountService, AccountService>();
        services.AddApiService<IProfileService, ProfileService>();
        services.AddApiService<ICategoryService, CategoryService>();
        services.AddApiService<IProductsService, ProductsService>();
        services.AddApiService<IAdministrationService, AdministrationService>();
        services.AddApiService<IBasketService, BasketService>();
        services.AddApiService<IOrderService, OrderService>();
        services.AddApiService<IPaymentService, PaymentService>();
        services.AddApiService<IRatingService, RatingService>();

        services.AddScoped<BasketStateService>();

        return services;
    }
}