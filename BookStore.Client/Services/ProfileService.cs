using System.Net.Http.Json;
using BookStore.Core.DTOs;

namespace BookStore.Client.Services;

public class ProfileService
{
    private readonly HttpClient _http;
    
    public ProfileService(HttpClient http)
    {
        _http = http;
    }

    public async Task<UserProfileDto?> GetProfileAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<UserProfileDto>("api/profile");
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public async Task<List<LibraryBookDto>> GetMyLibraryAsync()
    {
        return await _http.GetFromJsonAsync<List<LibraryBookDto>>("api/profile/library")
            ?? new List<LibraryBookDto>();
    }

    public async Task<List<BonusTransactionDto>> GetBonusHistoryAsync()
    {
        var result = await _http.GetFromJsonAsync<List<BonusTransactionDto>>("api/Profile/bonuses/history");
        return result ?? new List<BonusTransactionDto>();
    }

    public async Task<bool> UpdateProfileAsync(UserUpdateDto dto)
    {
        var response = await _http.PutAsJsonAsync("api/profile/update", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<string?> ChangePasswordAsync(ChangePasswordDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/profile/change-password", dto);
        if (response.IsSuccessStatusCode) return null;
        
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(); 
        return error?.Message ?? "Помилка зміни паролю";
    }
    
    class ErrorResponse { public string Message { get; set; } }
}