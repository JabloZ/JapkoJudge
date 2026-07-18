namespace WebBackend.Dto;
public class ChallengeViewDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public int Difficulty { get; set; }
    public string Description { get; set; } = "";
    public bool Verified { get; set; }
    public string Username { get; set; } = "";
}