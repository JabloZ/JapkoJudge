namespace WebBackend.Models;
using Microsoft.EntityFrameworkCore;
[Index(nameof(Username), IsUnique = true)]
[Index(nameof(Email), IsUnique = true)]
public class User
{
    public int Id{get;set;}
    public string Username{get;set;}=string.Empty;
    public string Email{get;set;}=string.Empty;
    public string PasswordHash{get;set;}=string.Empty;
    public DateTime CreatedAt{get;set;}=DateTime.UtcNow;
    public int TokenVersion{get;set;} =0;
    public bool Admin{get;set;} =false;
}