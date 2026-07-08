using WebBackend.Api;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
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
