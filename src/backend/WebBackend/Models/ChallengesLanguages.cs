using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
namespace WebBackend.Models;
public class ChallengeLanguage{
    public int Id{get;set;}
   
    public int ChallengeId{get;set;}

    [ForeignKey(nameof(ChallengeId))]
    public Challenge? Challenge{get;set;}

    public int LanguageId{get;set;}

    [ForeignKey(nameof(LanguageId))]
    public Language? Language{get;set;}

    public string StartCode{get;set;}="";
    public string TestfilePath{get;set;}="";
    public bool Verified{get;set;}
}