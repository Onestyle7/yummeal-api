using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Supabase.Gotrue;
using yummealAPI.Data;
using yummealAPI.Models.Dtos;
using yummealAPI.Models.Dtos.User;
using yummealAPI.Models.Entities;
using yummealAPI.Models.Enums;
using yummealAPI.Services.Interfaces;

namespace yummealAPI.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(
            ApplicationDbContext context,
            IMapper mapper,
            ILogger<UserService> logger
        )
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        #region Profile operations

        public async Task<UserProfileDto?> GetProfileAsync(string userId)
        {
            try
            {
                var profile = await _context
                    .UserProfiles.Include(p => p.Preferences)
                    .FirstOrDefaultAsync(p => p.UserId == userId);

                return profile == null ? null : _mapper.Map<UserProfileDto>(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user profile");
                throw;
            }
        }

        public async Task<UserProfileDto> CreateProfileAsync(
            string userId,
            CreateUserProfileDto createProfileDto
        )
        {
            try
            {
                // Check if profile already exists
                if (await ProfileExistsAsync(userId))
                {
                    throw new InvalidOperationException("Profile already exists for this user.");
                }

                var profile = _mapper.Map<UserProfile>(createProfileDto);
                profile.UserId = userId;
                profile.CreatedAt = DateTime.UtcNow;
                profile.UpdatedAt = DateTime.UtcNow;

                // Calculate nutrition goals if we have enough data
                if (
                    profile.Age > 0
                    && profile.Gender != null
                    && profile.Height > 0
                    && profile.CurrentWeight > 0
                    && profile.ActivityLevel != null
                )
                {
                    CalculateNutritionGoals(profile);
                }

                _context.UserProfiles.Add(profile);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created profile for user {UserId}", userId);
                return _mapper.Map<UserProfileDto>(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user profile for {UserId}", userId);
                throw;
            }
        }

        public async Task<UserProfileDto> UpdateProfileAsync(
            string userId,
            UpdateUserProfileDto updateProfileDto
        )
        {
            try
            {
                var profile = await _context.UserProfiles.FirstOrDefaultAsync(p =>
                    p.UserId == userId
                );

                if (profile == null)
                {
                    throw new InvalidOperationException($"Profile not found for user {userId}");
                }

                // Map only non-null values
                _mapper.Map(updateProfileDto, profile);
                profile.UpdatedAt = DateTime.UtcNow;

                // Recalculate nutrition if relevant data changed
                if (
                    updateProfileDto.Age.HasValue
                    || updateProfileDto.Gender.HasValue
                    || updateProfileDto.Height.HasValue
                    || updateProfileDto.CurrentWeight.HasValue
                    || updateProfileDto.ActivityLevel.HasValue
                    || updateProfileDto.WeightGoal.HasValue
                )
                {
                    CalculateNutritionGoals(profile);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated profile for user {UserId}", userId);
                return _mapper.Map<UserProfileDto>(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile for user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> DeleteProfileAsync(string userId)
        {
            try
            {
                var profile = await _context.UserProfiles.FirstOrDefaultAsync(p =>
                    p.UserId == userId
                );

                if (profile == null)
                {
                    return false;
                }

                _context.UserProfiles.Remove(profile);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted profile for user {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting profile for user {UserId}", userId);
                throw;
            }
        }
        #endregion
        #region Preferences operations

        public async Task<UserPreferencesDto?> GetPreferencesAsync(string userId)
        {
            try
            {
                var profile = await _context
                    .UserProfiles.Include(p => p.Preferences)
                    .FirstOrDefaultAsync(p => p.UserId == userId);

                return profile?.Preferences == null
                    ? null
                    : _mapper.Map<UserPreferencesDto>(profile.Preferences);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting preferences for user {UserId}", userId);
                throw;
            }
        }

        public async Task<UserPreferencesDto> CreateOrUpdatePreferencesAsync(
            string userId,
            UserPreferencesDto preferencesDto
        )
        {
            try
            {
                var profile = await _context
                    .UserProfiles.Include(p => p.Preferences)
                    .FirstOrDefaultAsync(p => p.UserId == userId);

                if (profile == null)
                {
                    throw new InvalidOperationException($"Profile not found for user {userId}");
                }

                if (profile.Preferences == null)
                {
                    // Create new preferences
                    var newPreferences = _mapper.Map<UserPreferences>(preferencesDto);
                    newPreferences.UserProfileId = profile.Id;
                    newPreferences.CreatedAt = DateTime.UtcNow;
                    newPreferences.UpdatedAt = DateTime.UtcNow;

                    _context.UserPreferences.Add(newPreferences);
                    profile.Preferences = newPreferences;
                }
                else
                {
                    // Update existing preferences
                    _mapper.Map(preferencesDto, profile.Preferences);
                    profile.Preferences.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated preferences for user {UserId}", userId);
                return _mapper.Map<UserPreferencesDto>(profile.Preferences);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating preferences for user {UserId}", userId);
                throw;
            }
        }
        #endregion

        #region OnBoarding
        public async Task<bool> CompleteOnboardingAsync(string userId)
        {
            try
            {
                var profile = await _context.UserProfiles.FirstOrDefaultAsync(p =>
                    p.UserId == userId
                );

                if (profile == null)
                {
                    return false;
                }

                profile.IsOnboardingCompleted = true;
                profile.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Completed onboarding for user {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing onboarding for user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> IsOnboardingCompletedAsync(string userId)
        {
            try
            {
                var profile = await _context.UserProfiles.FirstOrDefaultAsync(p =>
                    p.UserId == userId
                );

                return profile?.IsOnboardingCompleted ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking onboarding status for user {UserId}", userId);
                throw;
            }
        }
        #endregion

        #region Nutrition calculations
        public async Task<UserProfileDto> RecalculateNutritionGoalsAsync(string userId)
        {
            try
            {
                var profile = await _context.UserProfiles.FirstOrDefaultAsync(p =>
                    p.UserId == userId
                );

                if (profile == null)
                {
                    throw new InvalidOperationException($"Profile not found for user {userId}");
                }

                CalculateNutritionGoals(profile);
                profile.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Recalculated nutrition goals for user {UserId}", userId);
                return _mapper.Map<UserProfileDto>(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error recalculating nutrition goals for user {UserId}",
                    userId
                );
                throw;
            }
        }

        private void CalculateNutritionGoals(UserProfile profile)
        {
            if (
                profile.Age <= 0
                || !profile.Gender.HasValue
                || profile.Height <= 0 // int, nie int?
                || profile.CurrentWeight <= 0 // int, nie int?
                || !profile.ActivityLevel.HasValue
            )
            {
                return;
            }

            // ✅ POPRAWNIE - bez .Value bo to int
            var bmr =
                profile.Gender.Value == Gender.Male
                    ? (10 * (double)profile.CurrentWeight)
                        + (6.25 * profile.Height)
                        - (5 * profile.Age)
                        + 5
                    : (10 * (double)profile.CurrentWeight)
                        + (6.25 * profile.Height)
                        - (5 * profile.Age)
                        - 161;

            profile.BMR = (decimal)bmr;

            var activityMultiplier = profile.ActivityLevel.Value switch
            {
                ActivityLevel.Sedentary => 1.2,
                ActivityLevel.LightlyActive => 1.375,
                ActivityLevel.ModeratelyActive => 1.55,
                ActivityLevel.VeryActive => 1.725,
                ActivityLevel.SuperActive => 1.9,
                _ => 1.2,
            };

            var tdee = bmr * activityMultiplier;
            profile.TDEE = (decimal)tdee;

            var calorieAdjustment = profile.WeightGoal switch
            {
                WeightGoal.Lose => -500,
                WeightGoal.Gain => 500,
                _ => 0,
            };

            // ✅ POPRAWNIE - DailyCaloricIntake w Entity
            profile.DailyCaloricIntake = (decimal)(tdee + calorieAdjustment);

            // ✅ POPRAWNIE
            var totalCalories = (int)profile.DailyCaloricIntake.Value;
            profile.DailyProteinGoal = (decimal)(totalCalories * 0.25 / 4);
            profile.DailyCarbsGoal = (decimal)(totalCalories * 0.45 / 4);
            profile.DailyFatGoal = (decimal)(totalCalories * 0.30 / 9);
        }

        #endregion
        #region Health Checks

        public async Task<bool> ProfileExistsAsync(string userId)
        {
            try
            {
                return await _context.UserProfiles.AnyAsync(p => p.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if profile exists for user {UserId}", userId);
                throw;
            }
        }

        #endregion
    }
}
