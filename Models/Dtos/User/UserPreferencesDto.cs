using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using yummealAPI.Models.Enums;

namespace yummealAPI.Models.Dtos
{
    public class UserPreferencesDto
    {
        public int Id { get; set; }
        public DietType DietType { get; set; }
        public List<string> Allergies { get; set; } = new List<string>();
        public List<string> DislikedIngredients { get; set; } = new List<string>();
        public List<string> PreferredCuisines { get; set; } = new List<string>();
        [Range(2, 8)]
        public int MealFrequency { get; set; }
        [Range(1, 14)]
        public int PlanningDays { get; set; }
        
        [Range(5, 300)]
        public int MaxCookingTime { get; set; }
        
        public DifficultyLevel PreferredDifficulty { get; set; }
        public int AverageBudget { get; set; } = 100; // Średni budżet na tydzień
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}