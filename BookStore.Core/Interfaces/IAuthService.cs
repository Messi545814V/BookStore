using System.Threading.Tasks;
using BookStore.Core.DTOs; // або перенести DTO в Core, якщо вони потрібні в бізнес-логіці

namespace BookStore.Core.Interfaces;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterDto dto);
    Task<string> LoginAsync(LoginDto dto);
}