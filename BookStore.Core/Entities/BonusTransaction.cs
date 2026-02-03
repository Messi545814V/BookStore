namespace BookStore.Core.Entities;

public class BonusTransaction
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    
    public DateTime Date { get; set; } = DateTime.UtcNow;
    
    public decimal Amount { get; set; } 
    
    public string Description { get; set; } = string.Empty;
    
    public int? OrderId { get; set; }
}