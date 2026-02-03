using System.Text;
using System.Text.Json.Serialization;
using BookStore.Core.Entities;
using BookStore.Core.Helpers;
using BookStore.Core.Interfaces;
using BookStore.Infrastructure.Data;
using BookStore.Infrastructure.Repositories;
using BookStore.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity; 
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// 1. Налаштування бази даних
builder.Services.AddDbContext<BookStoreContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


var myAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
        policy =>
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        });
});

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("Jwt:Key not found in configuration.");
}
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });


builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAssertion(_ => true)
        .Build();

    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAssertion(_ => true)
        .Build();
});

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<AuthorService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddHttpClient<NovaPoshtaService>();
builder.Services.AddHttpClient<LiqPayService>();
builder.Services.AddScoped<BonusService>();

// 6. Налаштування контролерів та JSON серіалізації
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Ігнорування циклічних посилань
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 👇 ВСТАВТЕ ЦЕЙ БЛОК ВІДРАЗУ ПІСЛЯ app.Build() 👇
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BookStoreContext>();

    // Завантажуємо ВСІ книги з авторами та категоріями
    var books = await context.Books
        .Include(b => b.Author)
        .Include(b => b.Category)
        .ToListAsync();

    bool anyChanged = false;

    foreach (var b in books)
    {
        // Формуємо правильний пошуковий рядок
        var newSearchString = TextNormalizer.Normalize(
            $"{b.Title} {b.Genre} {b.Author?.Name ?? ""} {b.Category?.Name ?? ""}"
        );

        // Якщо поточне значення відрізняється (або null), оновлюємо
        if (b.SearchNormalized != newSearchString)
        {
            b.SearchNormalized = newSearchString;
            anyChanged = true;
        }
    }

    if (anyChanged)
    {
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Database search index updated successfully!");
    }
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(myAllowSpecificOrigins); 

app.UseAuthentication();          
app.UseAuthorization();        

app.MapControllers();

app.Run();

