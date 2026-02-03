using BookStore.Core.Entities;

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<OrderItem> OrderItems { get; set; } = new();
    public string Status { get; set; } = "Processing"; // New, Pending, Processing, Shipped

    public User User { get; set; } = null!;

    // Контактні дані
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }

    // Доставка
    public string DeliveryMethod { get; set; }
    public string City { get; set; }
    public string CityRef { get; set; }
    public string Warehouse { get; set; } // Відділення / Поштомат

    // Кур'єр
    public string Address { get; set; }
    public string House { get; set; }
    public string Apartment { get; set; }

    // Оплата
    public string PaymentMethod { get; set; }

    // Коментар
    public string Comment { get; set; }
    
    
    
    public decimal BonusesUsed { get; set; } = 0;
    
    public string DeliveryStatus { get; set; }
}