using System.ComponentModel.DataAnnotations;

namespace BookStore.Core.DTOs;

public class ChangePasswordDto
{
    [Required(ErrorMessage = "Введіть старий пароль")]
    public string OldPassword { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Введіть новий пароль")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Пароль має містити мінімум 8 символів")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).+$",
        ErrorMessage = "Пароль має містити принаймні одну велику літеру, одну малу літеру, одну цифру та один спеціальний символ.")]
    public string NewPassword { get; set; } = string.Empty;

    [Compare("NewPassword", ErrorMessage = "Паролі не співпадають")]
    public string ConfirmPassword { get; set; } = string.Empty;
}