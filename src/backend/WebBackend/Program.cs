using WebBackend.Api;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebBackend.Migrations;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

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
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => //setting jwt for user session in web
{
    options.MapInboundClaims = false; //so username doesnt get changed in token
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer=true,//from trusted?
        ValidateAudience=true,//for this app?
        ValidateLifetime=true,//active?
        ValidateIssuerSigningKey=true,//signing correct?
        ValidIssuer=builder.Configuration["Jwt:Issuer"],
        ValidAudience=builder.Configuration["Jwt:Audience"],
        IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))//env variables, symmetric so cant be leaked   
    };
    options.Events=new JwtBearerEvents
    {
        OnTokenValidated=async (context) =>
        {
            var userIdClaim=context.Principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var tokenVersionClaim=context.Principal?.FindFirst("tokenVersion")?.Value;
            if (userIdClaim is null || tokenVersionClaim is null)
            {
                context.Fail("Invalid token");
                return;
            }
            var dbContext=context.HttpContext.RequestServices.GetRequiredService<JudgeDbContext>();
            var user = await dbContext.Users.FindAsync(int.Parse(userIdClaim));
            if (user is null || user.TokenVersion.ToString() != tokenVersionClaim)
            {
                context.Fail("Token revoked");
                return;
            }
        }
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
app.UseAuthentication();   //auth and auth so session tokens are properly sent, order important
app.UseAuthorization();    
app.MapRegisterEndpoint(); //adding url endpoints 
app.MapLoginEndpoint();
app.MapChallengesEndpoint();
app.Run();
