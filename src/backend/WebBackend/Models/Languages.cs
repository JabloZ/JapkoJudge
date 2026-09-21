using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
namespace WebBackend.Models;
[Index(nameof(Extension), IsUnique = true)]
[Index(nameof(Name), IsUnique = true)]
public class Language{
    public int Id{get;set;}
   
    public string Name{get;set;}="";
    public string Extension{get;set;}="";
}