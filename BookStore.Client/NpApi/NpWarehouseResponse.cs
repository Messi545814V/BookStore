namespace BookStore.Client.NpApi;

public class NpWarehouseResponse
{
    public bool Success { get; set; }
    public List<WareHouseNp> data { get; set; }
}

public class WareHouseNp
{
    public string Description {get; set; }
}