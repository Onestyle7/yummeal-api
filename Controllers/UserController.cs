using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using yummealAPI.Models.Dtos;
using yummealAPI.Models.Dtos.User;
using yummealAPI.Services.Interfaces;

namespace yummealAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("User Management")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        #region Profile Endpoints

        /// <summary>
        /// Pobierz profil zalogowanego użytkownika
        /// </summary>
        [HttpGet("profile")]
        [Tags("User Profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                if (!User.Identity?.IsAuthenticated == true)
                {
                    return Unauthorized(new { message = "Użytkownik nie jest zalogowany" });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(
                        new { message = "Nie można pobrać ID użytkownika z tokena" }
                    );
                }

                var profile = await _userService.GetProfileAsync(userId);

                if (profile == null)
                {
                    return NotFound(new { message = "Profil użytkownika nie został znaleziony" });
                }

                return Ok(new { message = "Profil pobrany pomyślnie", data = profile });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania profilu użytkownika");
                return StatusCode(500, new { message = "Wystąpił błąd serwera" });
            }
        }

        /// <summary>
        /// Stwórz profil dla zalogowanego użytkownika
        /// </summary>
        [HttpPost("profile")]
        [Tags("User Profile")]
        public async Task<IActionResult> CreateProfile([FromBody] CreateUserProfileDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (!User.Identity?.IsAuthenticated == true)
                {
                    return Unauthorized(new { message = "Użytkownik nie jest zalogowany" });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(
                        new { message = "Nie można pobrać ID użytkownika z tokena" }
                    );
                }

                var profile = await _userService.CreateProfileAsync(userId, createDto);

                return CreatedAtAction(
                    nameof(GetProfile),
                    new { },
                    new { message = "Profil utworzony pomyślnie", data = profile }
                );
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas tworzenia profilu użytkownika");
                return StatusCode(500, new { message = "Wystąpił błąd serwera" });
            }
        }

        /// <summary>
        /// Aktualizuj profil zalogowanego użytkownika
        /// </summary>
        [HttpPut("profile")]
        [Tags("User Profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (!User.Identity?.IsAuthenticated == true)
                {
                    return Unauthorized(new { message = "Użytkownik nie jest zalogowany" });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(
                        new { message = "Nie można pobrać ID użytkownika z tokena" }
                    );
                }

                var updatedProfile = await _userService.UpdateProfileAsync(userId, updateDto);

                return Ok(
                    new { message = "Profil zaktualizowany pomyślnie", data = updatedProfile }
                );
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas aktualizacji profilu użytkownika");
                return StatusCode(500, new { message = "Wystąpił błąd serwera" });
            }
        }

        /// <summary>
        /// Usuń profil zalogowanego użytkownika
        /// </summary>
        [HttpDelete("profile")]
        [Tags("User Profile")]
        public async Task<IActionResult> DeleteProfile()
        {
            try
            {
                if (!User.Identity?.IsAuthenticated == true)
                {
                    return Unauthorized(new { message = "Użytkownik nie jest zalogowany" });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(
                        new { message = "Nie można pobrać ID użytkownika z tokena" }
                    );
                }

                var deleted = await _userService.DeleteProfileAsync(userId);

                if (!deleted)
                {
                    return NotFound(new { message = "Profil użytkownika nie został znaleziony" });
                }

                return Ok(new { message = "Profil usunięty pomyślnie" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas usuwania profilu użytkownika");
                return StatusCode(500, new { message = "Wystąpił błąd serwera" });
            }
        }

        #endregion

        #region Preferences Endpoints

        /// <summary>
        /// Pobierz preferencje zalogowanego użytkownika
        /// </summary>
        [HttpGet("preferences")]
        [Tags("User Preferences")]
        public async Task<IActionResult> GetPreferences()
        {
            try
            {
                if (!User.Identity?.IsAuthenticated == true)
                {
                    return Unauthorized(new { message = "Użytkownik nie jest zalogowany" });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(
                        new { message = "Nie można pobrać ID użytkownika z tokena" }
                    );
                }

                var preferences = await _userService.GetPreferencesAsync(userId);

                if (preferences == null)
                {
                    return NotFound(
                        new { message = "Preferencje użytkownika nie zostały znalezione" }
                    );
                }

                return Ok(new { message = "Preferencje pobrane pomyślnie", data = preferences });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania preferencji użytkownika");
                return StatusCode(500, new { message = "Wystąpił błąd serwera" });
            }
        }

        /// <summary>
        /// Utwórz lub zaktualizuj preferencje zalogowanego użytkownika
        /// </summary>
        [HttpPut("preferences")]
        [Tags("User Preferences")]
        public async Task<IActionResult> CreateOrUpdatePreferences(
            [FromBody] UserPreferencesDto preferencesDto
        )
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (!User.Identity?.IsAuthenticated == true)
                {
                    return Unauthorized(new { message = "Użytkownik nie jest zalogowany" });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(
                        new { message = "Nie można pobrać ID użytkownika z tokena" }
                    );
                }

                var preferences = await _userService.CreateOrUpdatePreferencesAsync(
                    userId,
                    preferencesDto
                );

                return Ok(
                    new { message = "Preferencje zaktualizowane pomyślnie", data = preferences }
                );
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas aktualizacji preferencji użytkownika");
                return StatusCode(500, new { message = "Wystąpił błąd serwera" });
            }
        }

        #endregion

        #region Onboarding Endpoints

        /// <summary>
        /// Sprawdź status onboardingu zalogowanego użytkownika
        /// </summary>
        [HttpGet("onboarding/status")]
        public async Task<IActionResult> GetOnboardingStatus()
        {
            try
            {
                if (!User.Identity?.IsAuthenticated == true)
                {
                    return Unauthorized(new { message = "Użytkownik nie jest zalogowany" });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(
                        new { message = "Nie można pobrać ID użytkownika z tokena" }
                    );
                }

                var isCompleted = await _userService.IsOnboardingCompletedAsync(userId);

                return Ok(
                    new
                    {
                        message = "Status onboardingu pobrany pomyślnie",
                        data = new { isOnboardingCompleted = isCompleted },
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania statusu onboardingu");
                return StatusCode(500, new { message = "Wystąpił błąd serwera" });
            }
        }

        /// <summary>
        /// Oznacz onboarding jako ukończony dla zalogowanego użytkownika
        /// </summary>
        [HttpPost("onboarding/complete")]
        public async Task<IActionResult> CompleteOnboarding()
        {
            try
            {
                if (!User.Identity?.IsAuthenticated == true)
                {
                    return Unauthorized(new { message = "Użytkownik nie jest zalogowany" });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(
                        new { message = "Nie można pobrać ID użytkownika z tokena" }
                    );
                }

                var completed = await _userService.CompleteOnboardingAsync(userId);

                if (!completed)
                {
                    return NotFound(new { message = "Profil użytkownika nie został znaleziony" });
                }

                return Ok(new { message = "Onboarding ukończony pomyślnie" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas ukończania onboardingu");
                return StatusCode(500, new { message = "Wystąpił błąd serwera" });
            }
        }

        #endregion

        #region Nutrition Endpoints

        /// <summary>
        /// Przelicz cele żywieniowe dla zalogowanego użytkownika
        /// </summary>
        [HttpPost("nutrition/recalculate")]
        public async Task<IActionResult> RecalculateNutritionGoals()
        {
            try
            {
                if (!User.Identity?.IsAuthenticated == true)
                {
                    return Unauthorized(new { message = "Użytkownik nie jest zalogowany" });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(
                        new { message = "Nie można pobrać ID użytkownika z tokena" }
                    );
                }

                var updatedProfile = await _userService.RecalculateNutritionGoalsAsync(userId);

                return Ok(
                    new { message = "Cele żywieniowe przeliczone pomyślnie", data = updatedProfile }
                );
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas przeliczania celów żywieniowych");
                return StatusCode(500, new { message = "Wystąpił błąd serwera" });
            }
        }

        #endregion

        #region Health Check Endpoints

        /// <summary>
        /// Sprawdź czy zalogowany użytkownik ma profil
        /// </summary>
        [HttpGet("profile/exists")]
        public async Task<IActionResult> CheckProfileExists()
        {
            try
            {
                if (!User.Identity?.IsAuthenticated == true)
                {
                    return Unauthorized(new { message = "Użytkownik nie jest zalogowany" });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(
                        new { message = "Nie można pobrać ID użytkownika z tokena" }
                    );
                }

                var exists = await _userService.ProfileExistsAsync(userId);

                return Ok(
                    new
                    {
                        message = "Status profilu sprawdzony pomyślnie",
                        data = new { profileExists = exists },
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas sprawdzania istnienia profilu");
                return StatusCode(500, new { message = "Wystąpił błąd serwera" });
            }
        }

        #endregion
    }
}
