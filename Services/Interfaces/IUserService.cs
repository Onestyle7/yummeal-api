using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using yummealAPI.Models.Dtos;
using yummealAPI.Models.Dtos.User;
using yummealAPI.Models.Entities;

namespace yummealAPI.Services.Interfaces
{
    public interface IUserService
    {
        // Profile operations
        Task<UserProfileDto?> GetProfileAsync(string userId);
        Task<UserProfileDto> CreateProfileAsync(
            string userId,
            CreateUserProfileDto createProfileDto
        );
        Task<UserProfileDto> UpdateProfileAsync(
            string userId,
            UpdateUserProfileDto updateProfileDto
        );
        Task<bool> DeleteProfileAsync(string userId);

        // Preferences operations
        Task<UserPreferencesDto?> GetPreferencesAsync(string userId);
        Task<UserPreferencesDto> CreateOrUpdatePreferencesAsync(
            string userId,
            UserPreferencesDto preferencesDto
        );

        // Onboarding

        Task<bool> CompleteOnboardingAsync(string userId);
        Task<bool> IsOnboardingCompletedAsync(string userId);

        // Nutrition calculations
        Task<UserProfileDto> RecalculateNutritionGoalsAsync(string userId);

        // Health checks
        Task<bool> ProfileExistsAsync(string userId);
    }
}
