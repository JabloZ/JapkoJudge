using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
namespace WebBackend.Models;
public class Challenge{
    public int Id{get;set;}
   
    public int OwnerId{get;set;}

    [ForeignKey(nameof(OwnerId))]
    public User? User{get;set;}
    public string Title{get;set;}="";
    public int Difficulty{get;set;}
    public string Description{get;set;}="";
}