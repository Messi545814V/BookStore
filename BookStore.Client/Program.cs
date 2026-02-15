using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BookStore.Client;
using BookStore.Client.Auth;
using BookStore.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<AuthHeaderHandler>();

builder.Services.AddHttpClient("Api", client =>
    {
        client.BaseAddress = builder.HostEnvironment.IsDevelopment() 
            ? new Uri("https://localhost:7125/") 
            : new Uri("https://bookstore-api-v2.runasp.net/");
    })
    .AddHttpMessageHandler<AuthHeaderHandler>();

builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api"));


builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<ClientBookService>();
builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<WishlistService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<WaitingListService>();

await builder.Build().RunAsync();











/*
 using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BookStore.Client;
using BookStore.Client.Auth;
using BookStore.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddTransient<AuthHeaderHandler>();

builder.Services.AddHttpClient("Api", client => 
        client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<AuthHeaderHandler>();

builder.Services.AddScoped(sp => new HttpClient 
{
    BaseAddress = new Uri("https://localhost:7125")
});

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<CartService>();

await builder.Build().RunAsync();
 */


/*
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BookStore.Client;
using BookStore.Client.Auth;
using BookStore.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// 1. Реєструємо наш обробник запитів, який буде додавати токен.
// Scoped важливий, бо він залежить від IJSRuntime.
builder.Services.AddScoped<AuthHeaderHandler>();

// 2. Налаштовуємо HttpClient, щоб він використовував наш обробник.
// Це найстабільніший спосіб реєстрації.
builder.Services.AddHttpClient("Api", client => 
        client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<AuthHeaderHandler>();

// 3. ВАЖЛИВО: Переконуємось, що коли хтось просить 'HttpClient', 
// він отримує саме наш налаштований клієнт з назвою "Api".
builder.Services.AddScoped(sp => 
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api"));

// 4. Реєструємо решту сервісів, як і раніше.
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CartService>();

await builder.Build().RunAsync();
*/