using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using yummealAPI.Models.Enums;

namespace yummealAPI.Models.Entities
{
    public class UserPreferences
    {
        public int Id { get; set; }
        public int UserProfileId { get; set; } // Identyfikator profilu użytkownika
        [ForeignKey("UserProfileId")]
        public UserProfile UserProfile { get; set; } = null!;// Nawigacja do profilu użytkownika 
        // Preferencje dotyczące diety
        public DietType DietType { get; set; } = DietType.Everything;

        // Alergie
        public List<string> Allergies { get; set; } = new List<string>();

        // Nielubiane składniki
        public List<string> DislikedIngredients { get; set; } = new List<string>();

        // Ulubione kuchnie
        public List<string> FavoriteCuisines { get; set; } = new List<string>();

        // Meal prep preferencje
        [Range(2, 8)]
        public int MelsPerDay { get; set; } = 3; // Liczba posiłków dziennie
        [Range(1, 14)]
        public int PlanningDays { get; set; } = 7; // Liczba dni planowania posiłków
        [Range(5, 300)]
        public int MaxCookingTime { get; set; } = 60; // Maksymalny czas gotowania w minutach
        public DifficultyLevel DifficultyLevel { get; set; } = DifficultyLevel.Easy; // Poziom trudności gotowania

        // Budżet
        [Range(0, 1000)]
        public decimal AverageBudget { get; set; } = 100; // Średni budżet na tydzień

        // Metadane
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Data utworzenia preferencji
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow; // Data ostatniej aktualizacji preferencji
    }
}