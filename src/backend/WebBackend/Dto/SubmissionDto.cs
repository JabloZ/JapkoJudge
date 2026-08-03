
namespace WebBackend.Dto;

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
    public string ChallengeTitle{get;set;}="";

}