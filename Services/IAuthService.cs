using AuthUPS.DTOs;

namespace AuthUPS.Services
{
    public interface IAuthService
    {
        Task<string> Registration(RegisterDTO registerDTO);
        Task<string> Login(LoginDTO loginDTO);
    }
}
