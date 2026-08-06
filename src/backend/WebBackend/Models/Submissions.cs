namespace WebBackend.Models;
using System.ComponentModel.DataAnnotations.Schema;
public class Submission
{
    public int Id{get;set;}
    public int OwnerId{get;set;}
    [ForeignKey(nameof(OwnerId))]
    public User? Owner{get;set;}

    public int ManifestId{get;set;}
    [ForeignKey(nameof(ManifestId))]
    public ChallengeLanguage? Manifest{get;set;}

    public string Code{get;set;}="";
    public string Status{get;set;}="";
    public string Message{get;set;}="";
    public int MemoryUsed{get;set;}
    public int ExecutionTime{get;set;}

}