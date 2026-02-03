namespace BookStore.Core.DTOs;

public class UserProfileDto
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string Surname { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }

    public int BonusBalance { get; set; } = 0;
    public string UserLevel { get; set; } = "Читач";
    
    public int OrderCount { get; set; }
    public int WishListCount { get; set; }
    
    public decimal TotalSpent { get; set; }
}