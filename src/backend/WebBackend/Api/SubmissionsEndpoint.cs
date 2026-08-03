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
using WebBackend.Code;
public static class SubmissionsEndpoint
{
    public static void MapSubmissionsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("api/submissions/add/challenge/{id}",async(int id, JudgeDbContext db, SubmissionAddDto dto,IConfiguration config,ClaimsPrincipal claims,DockerCodeRunner runner) =>
        {
            
            try
            {
                string code=dto.Code;
                string languageName=dto.LanguageName;
                var language=await db.Languages.FirstOrDefaultAsync(k=>k.Name==languageName);
                var manifest=await db.ChallengesLanguages.FirstOrDefaultAsync(k=>k.LanguageId==language.Id && k.ChallengeId==id);
                //manifest.TestfilePath to sciezka do pliku testowego w kontenerze judgebackend
                var uploadsRoot=config["FileStorage:UploadsPath"]!;
                
                var userId=claims.FindFirstValue(JwtRegisteredClaimNames.Sub);



                var submissionId = Guid.NewGuid();
                string relativeDir = Path.Combine("tmp", "submissions", submissionId.ToString());
                string submissionDir = Path.Combine(uploadsRoot, relativeDir);
                Directory.CreateDirectory(submissionDir);

                string solutionFileName = $"solution.{language.Extension}";
                string testFileName = $"test_solution.{language.Extension}";

                await File.WriteAllTextAsync(Path.Combine(submissionDir, solutionFileName), code);
                File.Copy(manifest.TestfilePath, Path.Combine(submissionDir, testFileName), overwrite: true);

                string pathTestRunnerContainer = $"/mnt/uploads/{relativeDir}/{testFileName}".Replace("\\", "/");
                var (image, cmd) = LanguageRunner.Get(languageName, pathTestRunnerContainer);

                var result = await runner.RunAsync(
                    relativeSubmissionPath: relativeDir,
                    image: image,
                    cmd: cmd,
                    timeoutSeconds: 10);
                Console.WriteLine($"stdout: {result.Stdout} stderr:{result.Stderr} cmd:{result.ExitCode}"); 
                var submission=new Submission();
                if(result.Stdout == "OK"){
                    submission.Message=result.Stdout;
                }
                else
                {
                    submission.Message=result.Stderr;
                }
                submission.OwnerId=Int32.Parse(userId);
                submission.ManifestId=manifest.Id;
                submission.Code=code;
                submission.Status=result.ExitCode.ToString();
                submission.MemoryUsed=(int)result.PeakMemoryBytes;
                submission.ExecutionTime=(int)result.ExecutionTimeMs;
                db.Submissions.Add(submission);
                await db.SaveChangesAsync();
                return Results.Ok(new
                {
                    
                    stdout = result.Stdout,
                    stderr = result.Stderr,
                    exitCode = result.ExitCode,
                    timedOut = result.TimedOut,
                    peakMemoryBytes = result.PeakMemoryBytes,
                    executionTimeMs = result.ExecutionTimeMs
                });
            }
            catch(Exception err)
            {
                return Results.BadRequest(new{message=$"Something went wrong: {err}"});
            }
           
            return Results.Ok(new{message="success"});
        }).RequireAuthorization();




        app.MapGet("api/submissions/get",async(JudgeDbContext db, ClaimsPrincipal claims)=>
        {
            var userId=claims.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var submissions=await db.Submissions
            .Select(k=>new SubmissionDto
            {
                Id=k.Id,
                OwnerId=k.OwnerId,
                ManifestId=k.ManifestId,
                Code=k.Code,
                Status=k.Status,
                Message=k.Message,
                MemoryUsed=k.MemoryUsed,
                ExecutionTime=k.ExecutionTime,
               
            })
            .Where(k=>k.OwnerId.ToString()==userId)
            .ToListAsync();
            foreach(SubmissionDto s in submissions)
            {
                var manifest=await db.ChallengesLanguages.FirstOrDefaultAsync(k=>k.Id==s.ManifestId);
                var challenge=await db.Challenges.FirstOrDefaultAsync(k=>k.Id==manifest.ChallengeId);
                s.ChallengeId=challenge.Id;
                s.ChallengeTitle=challenge.Title;
            }
            return Results.Ok(new{message="success",submissions});    
        }).RequireAuthorization();
    }
}
/*
public class SubmissionDto
{
    public int Id{get;set;}
    public int OwnerId{get;set;}
    public int ManifestId{get;set;}
    public string Code{get;set;}="";
    public string Status{get;set;}="";
    public string Message{get;set;}="";
    public int MemoryUsed{get;set;}
    public int ExecutionTime{get;set;}
    public int ChallengeId{get;set;}
    public string ChallengeName{get;set;}="";

}
*/