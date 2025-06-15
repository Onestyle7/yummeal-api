using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using yummealAPI.Models.Enums;

namespace yummealAPI.Models.Entities
{
    public class UserProfile
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string UserId { get; set; } = string.Empty;
        [StringLength(50)]
        public string? FirstName { get; set; }
        [StringLength(50)]
        public string? LastName { get; set; }
        [Range(13, 120)]
        public int Age { get; set; }
        public Gender? Gender { get; set; }
        [Range(100, 250)]
        public int Height { get; set; } // Wzrost w cm
        [Range(30, 300)]
        public int CurrentWeight { get; set; } // Waga w kg
        [Range(30, 300)]
        public int? TargetWeight { get; set; } // Docelowa waga w kg

        //Cele i aktywność
        public ActivityLevel? ActivityLevel { get; set; }
        public WeightGoal? WeightGoal { get; set; }

        //Obliczone wartości
        public decimal? BMR { get; set; } // Podstawowa przemiana materii
        public decimal? TDEE { get; set; } // Całkowita przemiana materii
        public decimal? DailyCaloricIntake { get; set; } // Dzienne zapotrzebowanie kaloryczne

        // Makroskładniki w gramach
        public decimal? DailyProteinGoal { get; set; } // Dzienne zapotrzebowanie na białko
        public decimal? DailyCarbsGoal { get; set; } // Dzienne zapotrzebowanie na węglowodany
        public decimal? DailyFatGoal { get; set; } // Dzienne zapotrzebowanie na tłuszcz

        // Metadane
        public bool IsOnboardingCompleted { get; set; } = false; // Czy onboarding został ukończony
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Data utworzenia profilu
        public DateTime? UpdatedAt { get; set; } // Data ostatniej aktualizacji profilu

        // Preferencje użytkownika
        public UserPreferences? UserPreferences { get; set; } // Preferencje użytkownika
    }
}