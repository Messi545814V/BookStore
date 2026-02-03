using System.ComponentModel.DataAnnotations;

namespace BookStore.Core.DTOs;

public class RegisterDto
{
    [Required(ErrorMessage = "Ім'я обов'язкове")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Ім'я користувача має бути від 3 до 50 символів.")]
    public string Username { get; set; } =  string.Empty;
    
    [Required(ErrorMessage = "Пароль є обов'язковим")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Пароль має містити мінімум 8 символів")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).+$",
        ErrorMessage = "Пароль має містити принаймні одну велику літеру, одну малу літеру, одну цифру та один спеціальний символ.")]
    public string Password { get; set; } =   string.Empty;
    
    [Required(ErrorMessage = "Підтвердження паролю є обов'язковим.")]
    [Compare("Password", ErrorMessage = "Пароль повинен співпадати.")]
    public string ConfirmPassword { get; set; } =   string.Empty;
}