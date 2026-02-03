namespace BookStore.Core.DTOs;

public class LiqPayCheckoutDto
{
    public string Data { get; set; } = "";
    public string Signature { get; set; } = "";
    public string Url { get; set; } = "https://www.liqpay.ua/api/3/checkout";
}