using System.Security.Cryptography;
using System.Text;
using System.Text.Json; // <--- ВАЖЛИВО: Використовуємо вбудований JSON
using BookStore.Core.DTOs;
using Microsoft.Extensions.Configuration;

namespace BookStore.Infrastructure.Services;

public class LiqPayService
{
    private readonly string _publicKey;
    private readonly string _privateKey;
    private readonly string _serverUrl;
    private readonly HttpClient _httpClient;

    public LiqPayService(IConfiguration config, HttpClient httpClient)
    {
        _publicKey = config["LiqPay:PublicKey"];
        _privateKey = config["LiqPay:PrivateKey"];
        _serverUrl = "https://localhost:7274";  
        _httpClient = httpClient;
    }

    public LiqPayCheckoutDto GetLiqPayRequest(int orderId, decimal amount)
    {
        var liqPayParams = new Dictionary<string, string>
        {
            { "version", "3" },
            { "public_key", _publicKey },
            { "action", "pay" },
            { "amount", amount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) },
            { "currency", "UAH" },
            { "description", $"Замовлення #{orderId}" },
            { "order_id", orderId.ToString() },
            { "language", "uk" },
            { "sandbox", "1" }, 
            { "result_url", $"{_serverUrl}/order-confirmation/{orderId}" }
        };
        
        var jsonString = JsonSerializer.Serialize(liqPayParams);

        var data = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonString));
        
        var signString = _privateKey + data + _privateKey;
        var signature = Convert.ToBase64String(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(signString)));

        return new LiqPayCheckoutDto
        {
            Data = data,
            Signature = signature,
            Url = "https://www.liqpay.ua/api/3/checkout" 
        };
    }

    public async Task<string> CheckStatus(string orderId)
    {
        var liqPayParams = new Dictionary<string, string>
        {
            { "action", "status" },
            { "version", "3" },
            { "public_key", _publicKey },
            { "order_id", orderId }
        };

        var jsonString = JsonSerializer.Serialize(liqPayParams);
        var data = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonString));
        
        var signString = _privateKey + data + _privateKey;
        var signature = Convert.ToBase64String(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(signString)));

        var requestContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("data", data),
            new KeyValuePair<string, string>("signature", signature),
        });
        
        var response = await _httpClient.PostAsync("https://www.liqpay.ua/api/request", requestContent);

        var responseString = await response.Content.ReadAsStringAsync();
        
        Console.WriteLine($"LiqPay API Response: {responseString}"); 

        try 
        {
            using var doc = JsonDocument.Parse(responseString);
            
            if (doc.RootElement.TryGetProperty("status", out var statusElement))
            {
                return statusElement.GetString();
            }
            if (doc.RootElement.TryGetProperty("result", out var resultElement))
            {
                 if (resultElement.GetString() == "error") return "failure";
            }
        }
        catch (JsonException)
        {
            Console.WriteLine("Помилка парсингу відповіді LiqPay (не JSON)");
            return "error";
        }

        return "unknown";
    }
}