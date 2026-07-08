using WebBackend.Api;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<JudgeDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Default"),
        npgsql => npgsql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)
    ));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNextJs", policy =>
    {
        policy.WithOrigins("http://judge-frontend:3000").AllowAnyHeader().AllowAnyMethod();     
    });
});
var app = builder.Build();
app.Urls.Add("http://0.0.0.0:8001");
app.UseCors("AllowNextJs");
app.MapRegisterEndpoint();
app.Run();
