using BookStore.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NovaPoshtaController : ControllerBase
{
    private readonly NovaPoshtaService _npService;

    public NovaPoshtaController(NovaPoshtaService npService)
    {
        _npService = npService;
    }

    [HttpGet("cities")]
    public async Task<IActionResult> SearchCities([FromQuery] string name)
    {
        Console.WriteLine($"[DEBUG] Отримано запит на пошук міста: '{name}'");
    
        if (string.IsNullOrWhiteSpace(name)) return BadRequest();
    
        var jsonResult = await _npService.SearchSettlements(name);
        return Content(jsonResult, "application/json");
    }

    [HttpGet("warehouses")]
    public async Task<IActionResult> GetWarehouses([FromQuery] string cityRef)
    {
        Console.WriteLine($"[DEBUG] Запит відділень для CityRef: '{cityRef}'");

        if (string.IsNullOrWhiteSpace(cityRef)) return BadRequest();

        var jsonResult = await _npService.GetWarehouses(cityRef);
        return Content(jsonResult, "application/json");
    }
}