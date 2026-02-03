using System.ComponentModel.DataAnnotations;

namespace BookStore.Core.DTOs;

public class LoginDto
{
    [Required(ErrorMessage = "Ім'я користувача обов'язково.")]
    public string Username { get; set; } =  string.Empty;
    
    [Required(ErrorMessage = "Пароль є обов'язковим.")]
    public string Password { get; set; } =  string.Empty;
}