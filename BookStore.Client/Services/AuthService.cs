using System.Net.Http.Json;
using BookStore.Client.Auth;
using BookStore.Core.DTOs;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace BookStore.Client.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    private readonly AuthenticationStateProvider  _authenticationStateProvider;

    public AuthService(HttpClient httpClient, IJSRuntime jsRuntime,  AuthenticationStateProvider authenticationStateProvider)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
        _authenticationStateProvider = authenticationStateProvider;
    }
    
    public async Task<(bool Success, string ErrorMessage)> RegisterAsync(RegisterDto registerDto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerDto);

        if (response.IsSuccessStatusCode)
        {
            return (true, String.Empty);
        }
        
        var errorMessage = await response.Content.ReadAsStringAsync();
        return (false, errorMessage);
    }

    public async Task<bool> LoginAsync(LoginDto  loginDto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginDto);

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var token = await response.Content.ReadAsStringAsync();
        await _jsRuntime.InvokeVoidAsync("localStorageFuncs.setItem", "authToken",  token);
        
        ((CustomAuthStateProvider)_authenticationStateProvider).NotifyUserAuthentication(token);
        
        return true;
    }

    public async Task LogoutAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorageFuncs.removeItem", "authToken");
        ((CustomAuthStateProvider)_authenticationStateProvider).NotifyUserLogout();
    }
}