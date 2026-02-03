namespace BookStore.Client.NpApi;

public class NpCityResponse
{
    public bool success { get; set; }
    public List<NpCityData> data { get; set; }
}

public class NpCityData
{
    public List<CityNp> Addresses { get; set; }
}

public class CityNp
{
    public string Ref { get; set; }
    
    // "Харків"
    public string MainDescription { get; set; } 
    
    // "м. Харків, Харківська обл." - це краще показувати у випадаючому списку
    public string Present { get; set; } 
    
    // В API може не бути поля Description, тому використовуємо Present
    public string Description => Present; 
}