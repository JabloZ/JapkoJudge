using WebBackend.Dto;
using WebBackend.Models;
namespace WebBackend.Api;
using WebBackend.Globals;
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
using WebBackend.Code;
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
                if (title.Length >= Globals.ChallengeTitle)
                {
                    return Results.BadRequest(new{message=$"Title length should be maximum {Globals.ChallengeTitle} characters long."});
                }
                if (description.Length >= Globals.ChallengeTitle)
                {
                    return Results.BadRequest(new{message=$"Title length should be maximum {Globals.ChallengeDescription} characters long."});
                }
                var userId=claims.FindFirstValue(JwtRegisteredClaimNames.Sub);
                
                if (userId is null || !int.TryParse(userId, out var userIdClaim)){
                    return Results.BadRequest(new{message="User not found!"});
                }
                var challenges=await db.Challenges.Where(a=>a.OwnerId.ToString()==userId).ToListAsync();
                if (challenges.Count()>100)
                {
                    return Results.Conflict(new{message="Reached limit of challenges per user (100)"});
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
                return Results.BadRequest(new{message=$"Error!"});
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
                if (dto.Startfile.Length > Globals.Submission)
                {
                    return Results.BadRequest(new { message = $"Start file too large. Max size: {Globals.Submission/1024} KB" });
                }
                if (dto.Testfile.Length > Globals.ChallengeTest)
                {
                    return Results.BadRequest(new { message = $"Test file too large. Max size: {Globals.ChallengeTest/ 1024} KB" });
                }
                var manifest=new ChallengeLanguage();
                //todo - mapping with database record

                var Language = await db.Languages.FirstOrDefaultAsync(l => l.Extension == dto.Language);
                if (Language is null)
                {
                    return Results.NotFound(new { message = "Language not found" });
                }
                var alreadySupported = await db.ChallengesLanguages.AnyAsync(m => m.ChallengeId == id && m.LanguageId == Language.Id);
                if (alreadySupported)
                {
                    return Results.Conflict(new { message = "This language is already supported" });
                }
                manifest.LanguageId = Language.Id;
                manifest.ChallengeId = id;
                
                var uploadsRoot=config["FileStorage:UploadsPath"]!;//from env
                var challengeDir=Path.Combine(uploadsRoot,"challenges",id.ToString(),manifest.LanguageId.ToString());
                Directory.CreateDirectory(challengeDir); 
                
                var startExt=Path.GetExtension(dto.Startfile.FileName);
                var exists=await db.Languages.FirstOrDefaultAsync(l=>l.Extension==startExt);
                if (exists is null)
                {
                    return Results.NotFound(new{message="Extension not found in database"});
                }
                
                var startPath=Path.Combine(challengeDir,$"start{startExt}");//create start.*
                await using(var stream = File.Create(startPath))
                {
                    await dto.Startfile.CopyToAsync(stream);
                }
                manifest.StartCode=startPath;

                var testExt=Path.GetExtension(dto.Testfile.FileName);
                exists=await db.Languages.FirstOrDefaultAsync(l=>l.Extension==testExt);
                if (exists is null)
                {
                    return Results.NotFound(new{message="Extension not found in database"});
                }
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
                return Results.BadRequest(new{message=$"Error while creating: "});
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
                if (challenge.Title.Length >= Globals.ChallengeTitle)
                {
                    return Results.BadRequest(new{message=$"Title length should be maximum {Globals.ChallengeTitle} characters long."});
                }
                if (challenge.Description.Length >= Globals.ChallengeTitle)
                {
                    return Results.BadRequest(new{message=$"Title length should be maximum {Globals.ChallengeDescription} characters long."});
                }
                challenge.Verified=false;
                await db.SaveChangesAsync();
                return Results.Ok(new{message="Succesfully edited challenge!"});
            }
            catch(Exception err)
            {
                return Results.BadRequest(new{message=$"Error editting! "});
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
                
                if (dto.Startfile.Length > Globals.Submission)
                {
                    return Results.BadRequest(new { message = $"Start file too large. Max size: {Globals.Submission/1024} KB" });
                }
                if (dto.Testfile.Length > Globals.ChallengeTest)
                {
                    return Results.BadRequest(new { message = $"Test file too large. Max size: {Globals.ChallengeTest/ 1024} KB" });
                }
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
                var duplicateExists = await db.ChallengesLanguages.AnyAsync(m => m.ChallengeId == id && m.LanguageId == Language.Id && m.Id != manifest.Id);
                if (duplicateExists)
                {
                    return Results.Conflict(new { message = "This language is already supported" });
                }
                manifest.LanguageId=Language.Id;
                manifest.ChallengeId=id;
                var uploadsRoot=config["FileStorage:UploadsPath"]!;//from env
                var challengeDir=Path.Combine(uploadsRoot,"challenges",id.ToString(),manifest.LanguageId.ToString());
                Directory.CreateDirectory(challengeDir); 
                
                var startExt=Path.GetExtension(dto.Startfile.FileName);
                var exists=await db.Languages.FirstOrDefaultAsync(l=>l.Extension==startExt);
                if (exists is null)
                {
                    return Results.NotFound(new{message="Extension not found in database"});
                }
                var startPath=Path.Combine(challengeDir,$"start{startExt}");//create start.*
                await using(var stream = File.Create(startPath))
                {
                    await dto.Startfile.CopyToAsync(stream);
                }
                manifest.StartCode=startPath;

                var testExt=Path.GetExtension(dto.Testfile.FileName);
                exists=await db.Languages.FirstOrDefaultAsync(l=>l.Extension==testExt);
                if (exists is null) 
                {
                    return Results.NotFound(new{message="Extension not found in database"});
                }
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
                return Results.BadRequest(new{message=$"Error while creating: "});
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
                return Results.BadRequest(new{message=$"Couldnt remove language support"});
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
                return Results.BadRequest(new{message=$"Couldnt remove language support"});
            }
            
            
        }).RequireAuthorization().DisableAntiforgery();


        //admin
        app.MapPost("api/post/verify_challenge/{id}",async(string id,DecisionDto dto,JudgeDbContext db, ClaimsPrincipal claims) =>
        {
            var isAdmin = await AuthHelper.IsUserAdmin(claims, db);
            if (isAdmin != true)
            {
                return Results.Forbid();
            }
            var challenge=await db.Challenges.FirstOrDefaultAsync(k=>k.Id.ToString()==id);
            if (challenge is null)
            {
                return Results.NotFound(new{message="Challenge not found"});
            }
            if (dto.decision == true)
            {
                challenge.Verified=true;
                await db.SaveChangesAsync();
                return Results.Ok(new{message=$"Challenge {id} is now verified!"});
            }
            else
            {
                challenge.Verified=false;
                await db.SaveChangesAsync();
                return Results.Ok(new{message=$"Challenge {id} is now unverified!"});
            }

        }).RequireAuthorization();
        //admin
        app.MapPost("api/post/change_difficulty/{id}",async(string id,DifficultyDto dto,JudgeDbContext db, ClaimsPrincipal claims) =>
        {

            var isAdmin = await AuthHelper.IsUserAdmin(claims, db);
            if (isAdmin != true)
            {
                return Results.Forbid();
            }
            var challenge=await db.Challenges.FirstOrDefaultAsync(k=>k.Id.ToString()==id);
            if (challenge is null)
            {
                return Results.NotFound(new { message = "Challenge not found" });
            }
            if (dto.Difficulty < 1 || dto.Difficulty > 7)
            {
                return Results.BadRequest(new { message = "Invalid difficulty" });
            }
            challenge.Difficulty=dto.Difficulty;
            await db.SaveChangesAsync();
            return Results.Ok(new{message=$"Challenge {id} is now difficulty {dto.Difficulty}!"});
          

        }).RequireAuthorization();
        //

        app.MapGet("api/get/challenge/{id}/is_author",async(int id, JudgeDbContext db, ClaimsPrincipal claims) =>
        {
            try
            {
                var userId=claims.FindFirstValue(JwtRegisteredClaimNames.Sub);
                var challenge=await db.Challenges.FirstOrDefaultAsync(c=>c.Id==id);
                if (challenge is null)
                {
                    return Results.NotFound(new{message=$"Challenge with id ${id} doesnt exist"});
                }
                bool author=challenge.OwnerId.ToString()==userId;
                return Results.Ok(new{author=author});
            }
            catch(Exception ex)
            {
                return Results.BadRequest(new{message="err"});
            }
        }).RequireAuthorization();

        app.MapGet("api/get/unverified_challenges",async(JudgeDbContext db, ClaimsPrincipal claims) =>
        {//name is misleading - change of conception. this api returns all challenges, but in future it will be needed
            try
            {
                var challenges=await db.Challenges.ToListAsync();
                return Results.Ok(new{message="Success",Challenges=challenges});
            }
            catch(Exception err)
            {
                return Results.BadRequest(new {message=$"Couldnt get challenges!"});
            }
        }).RequireAuthorization();
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
                return Results.BadRequest(new {message=$"Couldnt get challenges!"});
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
        

            
        app.MapGet("api/challenges/{id}/returnLanguages", async(int id, JudgeDbContext db, ClaimsPrincipal claims) =>{
        try
        {
        var viewer = claims.FindFirstValue(JwtRegisteredClaimNames.Sub);

        var manifests = await db.ChallengesLanguages
            .Where(cl => cl.ChallengeId == id)
            .Join(db.Languages,
                cl => cl.LanguageId,
                l => l.Id,
                (cl, l) => new ManifestDto
                {
                    Id = cl.Id,
                    ChallengeId = cl.ChallengeId,
                    LanguageId = cl.LanguageId,
                    LanguageName = l.Name,
                    AuthorId = viewer
                })
            .ToListAsync();

            return Results.Ok(new { message = "Succesfully returned", Manifests = manifests });
        }
        catch (Exception err)
        {
            return Results.Problem(statusCode: 500, detail: "Something went wrong returning languages");
        }
        }).RequireAuthorization();



        app.MapGet("api/challenges/{id}/language/{language_id}/supportInfo", async(int id, int language_id, JudgeDbContext db) =>
        {
            
            try{
                var manifest = await db.ChallengesLanguages
                .Where(cl => cl.ChallengeId == id && cl.LanguageId == language_id)
                .Join(db.Languages,
                    cl => cl.LanguageId,
                    l => l.Id,
                    (cl, l) => new ManifestDto
                    {
                        Id = cl.Id,
                        ChallengeId = cl.ChallengeId,
                        LanguageId = cl.LanguageId,
                        LanguageName = l.Name
                    }).FirstOrDefaultAsync();

                if (manifest is null)
                {
                    return Results.NotFound(new { message = "Manifest not found" });
                }

                return Results.Ok(new { message = "Succesfully returned", Manifest = manifest });
            }
            catch(Exception err)
            {
                return Results.BadRequest(new{message=$"Error returning language support"});
            }
        }).RequireAuthorization();
        app.MapGet("api/languages",async(JudgeDbContext db) =>
        {
            try{
                var languages=await db.Languages.ToListAsync();
                return Results.Ok(new{languages=languages});
            }
            catch(Exception ex)
            {
                return Results.BadRequest(new{message=$"err {ex}"});
            }
        }).RequireAuthorization();
    } 
}