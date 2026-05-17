using CityService.Controllers;
using CityService.Process;
using CityService.Repository;
using CommonUse;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Logging.ClearProviders();
builder.Logging.AddConsole();


builder.Services.AddDbContext<CityDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICity, CityRepository>();
builder.Services.AddScoped<CityProcess>();

//builder.Services.AddScoped<TokenValidator>();
//builder.Services.AddControllers(options =>
//{
//    options.Filters.Add(new CustomAuthenticationAttribute());
//});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi


builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "City API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(policyConfig =>
{
    policyConfig.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
});

app.UseAuthorization();

app.MapControllers();

app.Run();
