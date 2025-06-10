using Microsoft.AspNetCore.Mvc;
using yummealAPI.Dtos;

namespace yummealapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly Supabase.Client _supabase;

        public AuthController(ILogger<AuthController> logger, Supabase.Client supabase)
        {
            _supabase = supabase;
            _logger = logger;
        }

        [HttpPost("test")]
        public async Task<IActionResult> Test()
        {
            try
            {
                var isInitialized = _supabase != null;
                return Ok(new
                {
                    message = "Supabase Client zainicjalizowany!",
                    isConnected = isInitialized,
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Błąd z Supabase Client",
                    error = ex.Message
                });
            }
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            try
            {
                var session = await _supabase.Auth.SignUp(request.Email, request.Password);

                if (session?.User == null)
                {
                    return BadRequest(new { message = "Rejestracja nie powiodła się" });
                }
                var response = new AuthResponse
                {
                    AccessToken = session.AccessToken ?? "",
                    RefreshToken = session.RefreshToken ?? "",
                    User = new UserInfo
                    {
                        Id = session.User.Id,
                        Email = session.User.Email ?? "",
                        CreatedAt = session.User.CreatedAt
                    },
                    ExpiresAt = DateTime.UtcNow.AddHours(1)
                };
                return Ok(new { message = "Rejestracja zakończona sukcesem", data = response });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas rejestracji użytkownika");
                return BadRequest(new { message = "Błąd rejestracji", error = ex.Message });
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> login(LoginRequest request)
        {
            try
            {
                var session = await _supabase.Auth.SignIn(request.Email, request.Password);
                if (session?.User == null)
                {
                    return BadRequest(new { message = "Nieprawidłowy email lub hasło" });
                }
                var response = new AuthResponse
                {
                    AccessToken = session.AccessToken ?? "",
                    RefreshToken = session.RefreshToken ?? "",
                    User = new UserInfo
                    {
                        Id = session.User.Id,
                        Email = session.User.Email ?? "",
                        CreatedAt = session.User.CreatedAt
                    },
                    ExpiresAt = DateTime.UtcNow.AddHours(1)
                };
                return Ok(new { message = "Logowanie pomyślne!", data = response });
            }catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas logowania użytkownika");
                return BadRequest(new { message = "Błąd logowania", error = ex.Message });
            }
        }
    }
}