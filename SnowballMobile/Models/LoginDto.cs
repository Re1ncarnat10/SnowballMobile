using System.ComponentModel.DataAnnotations;
namespace SnowballMobile.Models;

public class LoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [MinLength(6)]
    [MaxLength(50)]
    public string Password { get; set; }
}