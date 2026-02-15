using System.ComponentModel.DataAnnotations;

namespace BookStore.Core.DTOs;

public class UserUpdateDto
{
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Ім'я користувача має бути від 2 до 50 символів.")]
    public string FullName { get; set; } =  string.Empty;
    
    [StringLength(50, ErrorMessage = "Прізвище користувача має бути від 2 до 50 символів.")]
    public string Surname { get; set; } =  string.Empty;
    
    [RegularExpression(@"^\+380\d{9}$", ErrorMessage = "Формат: +380XXXXXXXXX (рівно 12 цифр)")]
    public string Phone { get; set; } = "";
    
    public string Email { get; set; } = "";
}