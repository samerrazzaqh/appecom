using AppProject.API.Mapping;
using AppProject.API.Middleware;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();


// Add services to the container.
builder.Services.infrastructureConfiguration(builder.Configuration);
builder.Services.AddMemoryCache();
builder.Services.AddControllers();

// Swagger setup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(cfg => 
{
    cfg.AddProfile<OrderMapping>();
    cfg.AddProfile<CategoryMapping>();
    cfg.AddProfile<ProductMapping>();
});
// builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("CORSPolicy",
        builder =>
        {
            builder.WithOrigins("http://localhost:6001", "https://localhost:6000","https://samer.runasp.net","https://samer-kubernetes.infra-x.cloud")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});











var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

// Enable CORS
app.UseCors("CORSPolicy");

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Custom exception handling middleware
app.UseMiddleware<ExceptionsMiddleware>();

app.UseStaticFiles();
app.MapControllers();



app.Run();
