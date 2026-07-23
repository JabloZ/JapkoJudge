
namespace WebBackend.Dto;
public class ManifestDto
{
    public int Id { get; set; }
    public int ChallengeId{get;set;}
    public int LanguageId{get;set;}
    public string StartCode { get; set; }="";
    public string TestfilePath { get; set; } = "";
    public string LanguageName{get;set;}="";
    public string AuthorId{get;set;}="";
}