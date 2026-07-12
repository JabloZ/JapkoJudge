using WebBackend.Api;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
        policy.WithOrigins(builder.Configuration["FRONTEND_URL"]).AllowAnyHeader().AllowAnyMethod();     
    });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.MapInboundClaims = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer=true,
        ValidateAudience=true,
        ValidateLifetime=true,
        ValidateIssuerSigningKey=true,
        ValidIssuer=builder.Configuration["Jwt:Issuer"],
        ValidAudience=builder.Configuration["Jwt:Audience"],
        IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))   
    };
});

builder.Services.AddAuthorization();
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<JudgeDbContext>();
    db.Database.Migrate();
}
app.Urls.Add("http://0.0.0.0:8001"); //i added this port because .net automatically changed it to default from appsettings.json
app.UseCors("AllowNextJs");
app.UseAuthentication();   
app.UseAuthorization();    
app.MapRegisterEndpoint(); //adding url endpoints 
app.MapLoginEndpoint();
app.Run();
