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
using System.IO;
public static class ChallengesEndpoint
{
    public static void MapChallengesEndpoint(this IEndpointRouteBuilder app) 
    {
        //app.MapPost("api/create")
        //app.MapGet("api/read")
        //app.MapPost("api/update")
        //app.MapPost("api/delete")
    
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
            
            var challenge=await db.Challenges.FirstOrDefaultAsync(k=>k.Id==id);
            if (challenge==null)
            {
                return Results.BadRequest(new{message="Challenge with this id doesnt exists"});
            }
            if (challenge.OwnerId.ToString() != claims.FindFirstValue(JwtRegisteredClaimNames.Sub))
            {
                return Results.BadRequest(new{message="You are not an author!"});
            }
            try{

                var manifest=new ChallengeLanguage();
                //todo - mapping with database record

                var Language=await db.Languages.FirstOrDefaultAsync(l=>l.Extension==dto.Language);//not find async beacause not by id
                if (Language is null)
                {
                    return Results.NotFound(new{message="Language not found"});
                }
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
        
        app.MapPost("api/challenges/{id}/editGeneral",async(string id, [FromForm] ChallengeDto dto,JudgeDbContext db,IConfiguration config, ClaimsPrincipal claims) =>
        {
            
            try
            {
                var challenge=await db.Challenges.FirstOrDefaultAsync(k=>k.Id.ToString()==id);
                if (challenge is null)
                {
                    return Results.NotFound(new{message="Challenge not found"});
                }
                if (challenge.OwnerId.ToString() != claims.FindFirstValue(JwtRegisteredClaimNames.Sub))
                {
                    return Results.BadRequest(new{message="You are not an author!"});
                }
                challenge.Title=dto.Title;
                challenge.Description=dto.Description;
                challenge.Verified=false;
                await db.SaveChangesAsync();
                return Results.Ok(new{message="Succesfully edited challenge!"});
            }
            catch(Exception err)
            {
                return Results.BadRequest(new{message=$"Error editting! {err}"});
            }
        }).RequireAuthorization().DisableAntiforgery();
        //}/api/challenges/${id}/editLanguage/${language_id}
        app.MapPost("api/challenges/{id}/editLanguage/{language_id}",async(int id, int language_id,[FromForm] LanguageDto dto, ClaimsPrincipal claims, JudgeDbContext db, IConfiguration config) =>
        {
            //todo - delete old 
            var challenge=await db.Challenges.FirstOrDefaultAsync(k=>k.Id==id);
            if (challenge==null)
            {
                return Results.BadRequest(new{message="Challenge with this id doesnt exists"});
            }
            if (challenge.OwnerId.ToString() !=  claims.FindFirstValue(JwtRegisteredClaimNames.Sub))
            {
                return Results.BadRequest(new{message="You are not an author!"});
            }
            try{

                var manifest=await db.ChallengesLanguages.FirstOrDefaultAsync(k=>k.ChallengeId==id && k.LanguageId==language_id);
                //todo - mapping with database record
                if (manifest is null)
                {
                    return Results.NotFound(new{message="Manifest not found"});
                }
                string startcodePath=manifest.StartCode; 
                if (File.Exists(startcodePath))
                {
                    File.Delete(startcodePath);
                }
                string testcodePath=manifest.TestfilePath; 
                if (File.Exists(testcodePath))
                {
                    File.Delete(testcodePath);
                }
                var Language=await db.Languages.FirstOrDefaultAsync(l=>l.Id.ToString()==dto.Language);
                if (Language is null)
                {
                    return Results.NotFound(new{message="Language not found"});
                }
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

                manifest.Verified=false;
                await db.SaveChangesAsync();

                return Results.Ok(new { message = "Language added!" });
            }
            catch(Exception err)
            {
                return Results.BadRequest(new{message=$"Error while creating: {err}"});
            }

        }).RequireAuthorization().DisableAntiforgery();
        app.MapPost("api/challenges/{id}/deleteLanguageSupport/{language_id}", async(int id, int language_id, JudgeDbContext db,ClaimsPrincipal claims)=>{
            
            try
            {
                var challenge=await db.Challenges.FirstOrDefaultAsync(k=>k.Id==id);
                if (challenge==null)
                {
                    return Results.BadRequest(new{message="Challenge with this id doesnt exists"});
                }
                if (challenge.OwnerId.ToString() !=  claims.FindFirstValue(JwtRegisteredClaimNames.Sub))
                {
                    return Results.BadRequest(new{message="You are not an author!"});
                }

                var manifest=await db.ChallengesLanguages.FirstOrDefaultAsync(k=>k.ChallengeId==id && k.LanguageId==language_id);
                if (manifest is null)
                {
                    return Results.NotFound(new{message="Manifest not found"});
                }
                 string startcodePath=manifest.StartCode; 
                if (File.Exists(startcodePath))
                {
                    File.Delete(startcodePath);
                }
                string testcodePath=manifest.TestfilePath; 
                if (File.Exists(testcodePath))
                {
                    File.Delete(testcodePath);
                }
                db.ChallengesLanguages.Remove(manifest);
                await db.SaveChangesAsync();
                return Results.Ok(new{message="Succesfully deleted"});
            }
            catch(Exception err)
            {
                var manifest=await db.ChallengesLanguages.FirstOrDefaultAsync(k=>k.ChallengeId==id && k.LanguageId==language_id);
                return Results.BadRequest(new{message=$"Couldnt remove language support {err}"});
            }
            
            
        }).RequireAuthorization().DisableAntiforgery();

        app.MapPost("api/challenges/{id}/deleteChallenge", async(int id, JudgeDbContext db,ClaimsPrincipal claims)=>{
           
            try
            {
                var challenge=await db.Challenges.FirstOrDefaultAsync(k=>k.Id==id);
                if (challenge==null)
                {
                    return Results.BadRequest(new{message="Challenge with this id doesnt exists"});
                }
                if (challenge.OwnerId.ToString() !=  claims.FindFirstValue(JwtRegisteredClaimNames.Sub))
                {
                    return Results.BadRequest(new{message="You are not an author!"});
                }

                var manifest=await db.ChallengesLanguages.Where(k=>k.ChallengeId==id).ToListAsync();
                foreach (ChallengeLanguage m in manifest)
                {
                    if (m is null)
                    {
                        return Results.NotFound(new{message="Manifest not found"});
                    }
                    string startcodePath=m.StartCode; 
                    if (File.Exists(startcodePath))
                    {
                        File.Delete(startcodePath);
                    }
                    string testcodePath=m.TestfilePath; 
                    if (File.Exists(testcodePath))
                    {
                        File.Delete(testcodePath);
                    }
                }
                db.Challenges.Remove(challenge);
                await db.SaveChangesAsync();
                return Results.Ok(new{message="Succesfully deleted"});
            }
            catch(Exception err)
            {
                return Results.BadRequest(new{message=$"Couldnt remove language support {err}"});
            }
            
            
        }).RequireAuthorization().DisableAntiforgery();





        app.MapGet("api/users/{username}/challenges",async(string username, JudgeDbContext db, ClaimsPrincipal claims,IConfiguration config) =>
        {
            
            try{
                
                var owner=await db.Users.FirstOrDefaultAsync(k=>k.Username==username);
                var viewerUsername=claims.FindFirstValue(JwtRegisteredClaimNames.UniqueName);
                if (owner is null)
                {
                    return Results.NotFound(new{message="Owner not found"});
                }
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
         app.MapGet("api/challenges/{id:int}", async(int id, JudgeDbContext db) =>
        {
          
            var challenge=await db.Challenges.FindAsync(id);
            if (challenge == null)
            {
                return Results.NotFound(new{message=$"Challenge with id ${id} doesnt exist"});
            }
            return Results.Ok(challenge);
        }).RequireAuthorization();
        
        app.MapGet("api/challenges/{id}/returnLanguages", async(int id, JudgeDbContext db,ClaimsPrincipal claims) =>
        {
            
            try{
                var manifests=await db.ChallengesLanguages
                .Where(k=>k.ChallengeId==id)
                .Select(k=>new ManifestDto
                {
                    Id=k.Id,
                    ChallengeId=k.ChallengeId,
                    LanguageId=k.LanguageId,
                    StartCode=k.StartCode,
                    TestfilePath=k.TestfilePath
                })
                .ToListAsync();
                var viewer=claims.FindFirstValue(JwtRegisteredClaimNames.Sub);
                foreach(ManifestDto dto in manifests)
                {
                    var language=await db.Languages.FirstOrDefaultAsync(k=>k.Id==dto.LanguageId);
                    if (language is null)
                    {
                        return Results.NotFound(new{message="Language not found"});
                    }
                    dto.AuthorId=viewer;
                    dto.LanguageName=language.Name;
                }
                return Results.Ok(new{message="Succesfully returned",Manifests=manifests});
            }
            catch(Exception err)
            {
                return Results.BadRequest(new{message=$"Error returning languages supported: {err}"});
            }
        }).RequireAuthorization();
        app.MapGet("api/challenges/{id}/language/{language_id}/supportInfo", async(int id, int language_id, JudgeDbContext db) =>
        {
            
            try{
                var manifest=await db.ChallengesLanguages.Where(k=>k.ChallengeId==id && k.LanguageId==language_id)
                .Select(k=>new ManifestDto
                {
                    Id=k.Id,
                    ChallengeId=k.ChallengeId,
                    LanguageId=k.LanguageId,
                    StartCode=k.StartCode,
                    TestfilePath=k.TestfilePath
                }).FirstOrDefaultAsync();
                if (manifest is null)
                {
                    return Results.NotFound(new{message="Manifest not found"});
                }
                var language=await db.Languages.FirstOrDefaultAsync(k=>k.Id==language_id);
                if (language is null)
                {
                    return Results.NotFound(new{message="Language not found"});
                }
                manifest.LanguageName=language.Name;
                return Results.Ok(new{message="Succesfully returned",Manifest=manifest});
            }
            catch(Exception err)
            {
                return Results.BadRequest(new{message=$"Error returning language support {err}"});
            }
        }).RequireAuthorization();
    } 
}