using WebBackend.Api;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

/*
//It adds dbcontext (in ef it like a session for db connection lets say) with npgsql based on connectionstring (in docker from .env, in dev from appsetting.Dev...)
*/
builder.Services.AddDbContext<JudgeDbContext>(options => 
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Default"),
        npgsql => npgsql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)
    ));

builder.Services.AddCors(options => //adds cors for client side components from nextjs
{
    options.AddPolicy("AllowNextJs", policy =>
    {
        policy.WithOrigins(Environment.GetEnvironmentVariable("FRONTEND_URL")).AllowAnyHeader().AllowAnyMethod();     
    });
});
var app = builder.Build();
app.Urls.Add("http://0.0.0.0:8001"); //i added this port because .net automatically changed it to default from appsettings.json
app.UseCors("AllowNextJs");
app.MapRegisterEndpoint(); //adding url endpoints 
app.Run();
