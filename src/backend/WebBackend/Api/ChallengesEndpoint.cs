using WebBackend.Dto;
using WebBackend.Models;
namespace WebBackend.Api;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

public static class ChallengesEndpoint
{
    public static void MapChallengesEndpoint(this IEndpointRouteBuilder app) 
    {
        //app.MapPost("api/create")
        //app.MapGet("api/read")
        //app.MapPost("api/update")
        //app.MapPost("api/delete")
        app.MapGet("api/challenges/{id:int}", async(int id, JudgeDbContext db) =>
        {
            var challenge=await db.Challenges.FindAsync(id);
            if (challenge == null)
            {
                return Results.NotFound(new{message=$"Challenge with id ${id} doesnt exist"});
            }
            return Results.Ok(challenge);
        }).RequireAuthorization();

        app.MapPost("api/createChallenge", async([FromForm] ChallengeDto dto, ClaimsPrincipal claims, JudgeDbContext db, IConfiguration config) =>
        {
            try{
                
                string title=dto.Title;
                string description=dto.Description;
                var userId=claims.FindFirstValue(JwtRegisteredClaimNames.Sub);
                
                if (userId is null || !int.TryParse(userId, out var userIdClaim)){
                    return Results.BadRequest(new{message="User not found!"});
                }

                var challenge=new Challenge
                {
                    OwnerId=userIdClaim,
                    Title=title,
                    Difficulty=0,
                    Description=description
                };
                db.Challenges.Add(challenge);
                await db.SaveChangesAsync();
                return Results.Ok(new{message="Challenge created!", challengeId=challenge.Id.ToString()});
            }
            catch (Exception err)
            {
                return Results.BadRequest(new{message=$"Error! {err}"});
            }
            //if error occures afterthis, you need to remove challenge from db
            //todo - endpoint for quering languages
            

            //tu chce wywolac apiaddlanguagetochallenge
            
            
        }).RequireAuthorization().DisableAntiforgery();
        app.MapPost("api/addLanguageToChallenge/{id}",async(int id, [FromForm] LanguageDto dto, JudgeDbContext db, IConfiguration config) =>
        {
            Console.WriteLine("aaa");
            var manifest=new ChallengeLanguage();
            //todo - mapping with database record
            if (dto.Language == "c")
            {
                manifest.LanguageId=1;
            }
            else
            {
                manifest.LanguageId=2;
            }
            manifest.ChallengeId=id;
            var uploadsRoot=config["FileStorage:UploadsPath"]!;//from env
            var challengeDir=Path.Combine(uploadsRoot,"challenges",id.ToString(),manifest.LanguageId.ToString());
            Directory.CreateDirectory(challengeDir); 
            
            var startExt=Path.GetExtension(dto.Startfile.FileName);
            var startPath=Path.Combine(challengeDir,$"start{startExt}");
            await using(var stream = File.Create(startPath))
            {
                await dto.Startfile.CopyToAsync(stream);
            }
            manifest.StartCode=startPath;

            var testExt=Path.GetExtension(dto.Testfile.FileName);
            var testPath=Path.Combine(challengeDir,$"test{testExt}");
            await using(var stream = File.Create(testPath))
            {
                await dto.Testfile.CopyToAsync(stream);
            }
            manifest.TestfilePath=testPath;

            db.ChallengesLanguages.Add(manifest);
            await db.SaveChangesAsync();

            return Results.Ok(new { message = "Language added!" });

        }).RequireAuthorization().DisableAntiforgery();
    } 

    
}