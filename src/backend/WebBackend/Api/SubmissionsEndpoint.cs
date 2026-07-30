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

public static class SubmissionsEndpoint
{
    public static void MapSubmissionsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("api/submissions/add/challenge/{id}",async(int id, SubmissionAddDto dto) =>
        {
            Console.WriteLine("afasgasg");
            try
            {
                string code=dto.Code;
                //string languageName=dto.LanguageName;
                Console.WriteLine(code);
            }
            catch(Exception err)
            {
                return Results.BadRequest(new{message=$"Something went wrong: {err}"});
            }
            return Results.Ok(new{message="success"});
        }).RequireAuthorization();
    }
}