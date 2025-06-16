using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using yummealAPI.Models.Enums;

namespace yummealAPI.Models.Dtos.User
{
    public class UserProfileDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? Age { get; set; }
        public Gender? Gender { get; set; }
        public int? Height { get; set; }
        public decimal? CurrentWeight { get; set; }
        public decimal? TargetWeight { get; set; }
        public WeightGoal? WeightGoal { get; set; }
        public ActivityLevel? ActivityLevel { get; set; }
        
        // Obliczone wartości
        public decimal? BMR { get; set; }
        public decimal? TDEE { get; set; }
        public int? DailyCalorieGoal { get; set; }
        public decimal? DailyProteinGoal { get; set; }
        public decimal? DailyCarbsGoal { get; set; }
        public decimal? DailyFatGoal { get; set; }
        
        public bool IsOnboardingCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Zagnieżdżone preferencje
        public UserPreferencesDto? Preferences { get; set; }
    }
}