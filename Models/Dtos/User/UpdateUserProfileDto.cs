using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using yummealAPI.Models.Enums;

namespace yummealAPI.Models.Dtos.User
{
    public class UpdateUserProfileDto
    {
        [StringLength(50)]
        public string? FirstName { get; set; }
        
        [StringLength(50)]
        public string? LastName { get; set; }
        
        [Range(13, 120)]
        public int? Age { get; set; }
        
        public Gender? Gender { get; set; }
        
        [Range(100, 250)]
        public int? Height { get; set; }
        
        [Range(30, 300)]
        public decimal? CurrentWeight { get; set; }
        
        [Range(30, 300)]
        public decimal? TargetWeight { get; set; }
        
        public WeightGoal? WeightGoal { get; set; }
        public ActivityLevel? ActivityLevel { get; set; }
    }
}