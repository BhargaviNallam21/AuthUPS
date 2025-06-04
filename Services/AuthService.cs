using AuthUPS.Data;
using AuthUPS.DTOs;
using AuthUPS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthUPS.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly ParcelTrackingDBContext _context;

        public AuthService(IConfiguration config, ParcelTrackingDBContext context)
        {
            _config = config;
            _context = context;
        }
        public async Task<string> Registration(RegisterDTO registerDTO)
        {
            if (await _context.users.AnyAsync(u => u.UserName == registerDTO.Username))
            {
                throw new Exception("User Name Exists");
            }
            if (await _context.users.AnyAsync(u => u.Email == registerDTO.Email))
            {
                throw new Exception("Mail already Exists");
            }

            using var sha = SHA256.Create();
            var PasswordHash = Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)));

            var user = new User
            {
                UserName = registerDTO.Username,
                Email = registerDTO.Email,
                PasswordHash = PasswordHash,
                Role = registerDTO.Role,
                //CreatedAt = DateTime.UtcNow
            };
            _context.users.Add(user);
            await _context.SaveChangesAsync();
            return "User Registered Successfully";
        }
        public async Task<string> Login(LoginDTO loginDTO)
        {
            var user = await _context.users.SingleOrDefaultAsync(u => u.UserName == loginDTO.Username);
            if (user == null)
            {
                throw new Exception("Invalid UserName ");

            }

            using var sha = SHA256.Create();
            var PasswordHash = Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password)));

            if (user.PasswordHash != PasswordHash)
            {
                throw new Exception("Invalid password");
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name,user.UserName),
                     new Claim(ClaimTypes.Role,user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)


            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);



        }


    }
}
