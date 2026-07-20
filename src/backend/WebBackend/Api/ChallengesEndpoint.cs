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
using WebBackend.Migrations;

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
            
        }).RequireAuthorization().DisableAntiforgery();//disable antiforgery is for fromform
        //fromform because of file input
        app.MapPost("api/addLanguageToChallenge/{id}",async(int id, [FromForm] LanguageDto dto, ClaimsPrincipal claims, JudgeDbContext db, IConfiguration config) =>
        {
            //todo:
            /*
            check if this challenge already has this language supported

            */
            //
            var challenge=await db.Challenges.FindAsync(id.ToString());
            if (challenge==null)
            {
                return Results.BadRequest(new{message="Challenge with this id doesnt exists"});
            }
            if (challenge.OwnerId.ToString() != JwtRegisteredClaimNames.Sub)
            {
                return Results.BadRequest(new{message="You are not an author!"});
            }
            try{
                var manifest=new ChallengeLanguage();
                //todo - mapping with database record

                var Language=await db.Languages.FirstOrDefaultAsync(l=>l.Extension==dto.Language);//not find async beacause not by id
                manifest.LanguageId=Language.Id;
                manifest.ChallengeId=id;
                var uploadsRoot=config["FileStorage:UploadsPath"]!;//from env
                var challengeDir=Path.Combine(uploadsRoot,"challenges",id.ToString(),manifest.LanguageId.ToString());
                Directory.CreateDirectory(challengeDir); 
                
                var startExt=Path.GetExtension(dto.Startfile.FileName);
                var startPath=Path.Combine(challengeDir,$"start{startExt}");//create start.*
                await using(var stream = File.Create(startPath))
                {
                    await dto.Startfile.CopyToAsync(stream);
                }
                manifest.StartCode=startPath;

                var testExt=Path.GetExtension(dto.Testfile.FileName);
                var testPath=Path.Combine(challengeDir,$"test{testExt}");//create test.*
                await using(var stream = File.Create(testPath))
                {
                    await dto.Testfile.CopyToAsync(stream);
                }
                manifest.TestfilePath=testPath;

                db.ChallengesLanguages.Add(manifest);
                await db.SaveChangesAsync();

                return Results.Ok(new { message = "Language added!" });
            }
            catch(Exception err)
            {
                return Results.BadRequest(new{message=$"Error while creating: {err}"});
            }

        }).RequireAuthorization().DisableAntiforgery();
        app.MapGet("api/users/{username}/challenges",async(string username, JudgeDbContext db, ClaimsPrincipal claims,IConfiguration config) =>
        {
            
            try{
                
                var owner=await db.Users.FirstOrDefaultAsync(k=>k.Username==username);
                var viewerUsername=claims.FindFirstValue(JwtRegisteredClaimNames.UniqueName);
                bool viewerOwner= owner.Username==viewerUsername;
                
                var challenges=await db.Challenges
                .Where(k=>k.OwnerId==owner.Id)
                .Select(k=>new ChallengeViewDto
                {
                    Id=k.Id,
                    Title=k.Title,
                    Username=k.User!.Username,
                    Difficulty=k.Difficulty,
                    Description=k.Description,
                    Verified=k.Verified,
                    ViewerOwner=viewerOwner
                })
                .ToListAsync();
                
                return Results.Ok(new{message="Challenges returned", Challenges=challenges});
            }
            catch(Exception err)
            {
                return Results.BadRequest(new {message=$"Couldnt get challenges! {err}"});
            }
        }).RequireAuthorization();
        app.MapPost("api/challenges/{id}/editGeneral",async(string id, [FromForm] ChallengeDto dto,JudgeDbContext db,IConfiguration config) =>
        {
            Console.WriteLine("aabbbb");
            try
            {
                var challenge=await db.Challenges.FirstOrDefaultAsync(k=>k.Id.ToString()==id);
                challenge.Title=dto.Title;
                challenge.Description=dto.Description;
                await db.SaveChangesAsync();
                return Results.Ok(new{message="Succesfully edited challenge!"});
            }
            catch(Exception err)
            {
                return Results.BadRequest(new{message=$"Error editting! {err}"});
            }
        }).RequireAuthorization().DisableAntiforgery();
    } 
}