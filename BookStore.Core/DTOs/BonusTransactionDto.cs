namespace BookStore.Core.DTOs;

public class BonusTransactionDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public int? OrderId { get; set; }
}