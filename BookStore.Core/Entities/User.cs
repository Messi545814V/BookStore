using System.ComponentModel.DataAnnotations;

namespace BookStore.Core.Entities;

public class User
{
    public int  Id { get; set; }
    
    [Required]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string Role { get; set; } = "User";
    
    public string FullName { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    
    public string PhoneNumber { get; set; } = string.Empty;
    
    public decimal BonusBalance { get; set; } = 0;
    
    public decimal TotalSpent { get; set; } = 0;

    public List<Order> Orders { get; set; } = new();
}