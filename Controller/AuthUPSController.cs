using AuthUPS.Data;
using AuthUPS.DTOs;
using AuthUPS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthUPS.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthUPSController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ParcelTrackingDBContext _dbContext;

        public AuthUPSController(IAuthService authService, ParcelTrackingDBContext dBContext)
        {
            _authService = authService;
            _dbContext = dBContext;
        }

        [HttpGet("getusers")]
        public async Task<IActionResult> getusers()
        {
            return Ok(await _dbContext.users.ToListAsync());

        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            try
            {
                var results = await _authService.Registration(dto);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            try
            {
                var result = await _authService.Login(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
