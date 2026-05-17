using AuthenticationService.EntityModel;
using AuthenticationService.Process;
using AuthenticationService.Repository;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(); var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services
    .AddOpenApi()
    .AddDbContext<UsersDbContext>(
        options => options.UseSqlServer(connStr)
    )
    .Configure<AppSettings>(
        builder.Configuration.GetSection("AppSettings")
    )
    .AddHttpContextAccessor()
    .AddScoped<IUserRepository, UsersRepository>()
    .AddScoped<AuthProcess>()
    .AddScoped<TokenManager>()
    .AddCors(policyConfig =>
    {
        policyConfig.AddPolicy("allowAllPolicy",
            policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
    })

    .AddSwaggerGen()
    .AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler();
}
app.UseCors("allowAllPolicy");

app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.Run();
