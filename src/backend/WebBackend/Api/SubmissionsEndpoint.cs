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
            Console.WriteLine("afasgasg");
            try
            {
                string code=dto.Code;
                string languageName=dto.LanguageName;
                var language=await db.Languages.FirstOrDefaultAsync(k=>k.Name==languageName);
                
                var uploadsRoot=config["FileStorage:UploadsPath"]!;
                
                var userId=claims.FindFirstValue(JwtRegisteredClaimNames.Sub);

                var submissionId = Guid.NewGuid();
                string relativeDir=Path.Combine("tmp","submissions",submissionId.ToString());
                string submissionDir = Path.Combine(uploadsRoot, "tmp", "submissions", submissionId.ToString());
                Directory.CreateDirectory(submissionDir);

                string fileName=$"solution.{language.Extension}";
                var submissionPath = Path.Combine(submissionDir,fileName);
                await File.WriteAllTextAsync(submissionPath,code);
                
                string pathInRunnerContainer = $"/mnt/uploads/{relativeDir}/{fileName}".Replace("\\", "/");
                var (image, cmd) = LanguageRunner.Get(languageName, pathInRunnerContainer);

                var result = await runner.RunAsync(
                    relativeSubmissionPath: relativeDir,
                    image: image,
                    cmd: cmd,
                    timeoutSeconds: 10);
                Console.WriteLine($"stdout: {result.Stdout} stderr:{result.Stderr} cmd:{result.ExitCode}"); 
                return Results.Ok(new
                {
                    stdout = result.Stdout,
                    stderr = result.Stderr,
                    exitCode = result.ExitCode,
                    timedOut = result.TimedOut
                });
            }
            catch(Exception err)
            {
                return Results.BadRequest(new{message=$"Something went wrong: {err}"});
            }
           
            return Results.Ok(new{message="success"});
        }).RequireAuthorization();
    }
}