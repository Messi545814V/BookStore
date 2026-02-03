using System.ComponentModel.DataAnnotations;

namespace BookStore.Core.DTOs;

public class CheckoutOrderDto : IValidatableObject
{
    [Required(ErrorMessage = "Введіть ім'я")]
    public string FirstName { get; set; } = "";

    [Required(ErrorMessage = "Введіть прізвище")]
    public string LastName { get; set; } = "";

    [Required(ErrorMessage = "Введіть Email")]
    [EmailAddress(ErrorMessage = "Некоректний формат Email, Приклад: user@example.com")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Введіть номер телефону")]
    [RegularExpression(@"^\+380\d{9}$", ErrorMessage = "Формат: +380XXXXXXXXX (рівно 12 цифр)")]
    public string Phone { get; set; } = "";
    
    [Required(ErrorMessage = "Оберіть спосіб доставки")]
    public string DeliveryMethod { get; set; } = "";

    [Required(ErrorMessage = "Оберіть місто зі списку")]
    public string City { get; set; } = "";

    // CityRef потрібен для бекенду, але користувач його не вводить руками, 
    // тому Required тут не обов'язковий для UI, але бажаний для логіки.
    public string CityRef { get; set; } = ""; 

    // Ці поля перевіряються внизу в методі Validate
    public string Warehouse { get; set; } = "";
    public string Address { get; set; } = "";
    public string House { get; set; } = "";
    public string Apartment { get; set; } = "";
    
    [Required(ErrorMessage = "Оберіть спосіб оплати")]
    public string PaymentMethod { get; set; } = "";
    public int BonusesToUse { get; set; } = 0;

    public string Comment { get; set; } = "";

    // КАСТОМНА ВАЛІДАЦІЯ (Логіка для Кур'єра або Відділення)
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Якщо обрано доставку на відділення або поштомат
        if (DeliveryMethod.Contains("Відділення") || DeliveryMethod.Contains("Поштомат"))
        {
            if (string.IsNullOrWhiteSpace(Warehouse))
            {
                yield return new ValidationResult(
                    "Оберіть відділення або поштомат.", 
                    new[] { nameof(Warehouse) });
            }
        }

        // Якщо обрано доставку кур'єром
        if (DeliveryMethod.Contains("Кур'єр"))
        {
            if (string.IsNullOrWhiteSpace(Address))
            {
                yield return new ValidationResult(
                    "Вкажіть вулицю для кур'єра.", 
                    new[] { nameof(Address) });
            }
            if (string.IsNullOrWhiteSpace(House))
            {
                yield return new ValidationResult(
                    "Вкажіть номер будинку.", 
                    new[] { nameof(House) });
            }
        }
    }
}