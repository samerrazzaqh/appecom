using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AppProject.Client;
using AppProject.Client.Helpers;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


// قراءة عنوان الـ Client الحقيقي تلقائياً من المتصفح
var clientAddress = builder.HostEnvironment.BaseAddress;

// قراءة عنوان الـ API من appsettings.json
var apiAddress = builder.Configuration["ApiSettings:BaseApiAddress"] ?? "http://localhost:5001/";
// var apiAddress = builder.Configuration["ApiSettings:BaseApiAddress"] ?? "https://samer-kubernetes.infra-x.cloud/";


// تحديث القيم في AppConstant
//AppConstant.BaseApiAddressClient = clientAddress;
AppConstant.BaseApiAddress = apiAddress;

builder.Services.infrastructureConfigurationClient(builder.Configuration);

await builder.Build().RunAsync();