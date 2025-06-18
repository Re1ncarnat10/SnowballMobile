using System.ComponentModel.DataAnnotations;

namespace SnowballMobile.Models;

public class RegisterDto
{
    [Required]
    [MinLength(2)]
    public string Name { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [MinLength(6), MaxLength(50)]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; }
}