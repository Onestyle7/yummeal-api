using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace yummealAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SecureTestController : ControllerBase
    {
        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return Unauthorized(new { message = "Token wymagany" });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var userClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();

            return Ok(
                new
                {
                    message = "Jesteś zalogowany",
                    UserId = userId,
                    Email = userEmail,
                    Claims = userClaims,
                }
            );
        }

        [HttpGet("data")]
        public IActionResult GetUserData()
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return Unauthorized(new { message = "Token wymagany" });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Ok(
                new
                {
                    message = $"Prywatne dane dla użytkownika {userId}",
                    data = new
                    {
                        favoriteRecipes = new[] { "Kotlet schabowy", "Pierogi" },
                        mealPlans = new[] { "Plan na styczeń", "Plan dieta keto" },
                    },
                    timestamp = DateTime.Now,
                }
            );
        }

        [HttpGet("public")]
        public IActionResult GetPublicData()
        {
            return Ok(
                new
                {
                    message = "To są publiczne dane - nie potrzebujesz tokena",
                    data = "Publiczna informacja",
                    timestamp = DateTime.Now,
                }
            );
        }

        [HttpGet("debug/headers")]
        [AllowAnonymous] // Bez autoryzacji, żeby sprawdzić co przychodzi
        public IActionResult DebugHeaders()
        {
            var headers = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();

            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();

            return Ok(
                new
                {
                    message = "Debug headers",
                    allHeaders = headers,
                    authorizationHeader = authHeader,
                    isAuthenticated = User.Identity?.IsAuthenticated,
                    claimsCount = claims.Count,
                    claims = claims,
                    timestamp = DateTime.Now,
                }
            );
        }
    }
}
