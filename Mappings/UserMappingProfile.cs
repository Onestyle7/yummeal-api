using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using yummealAPI.Models.Dtos;
using yummealAPI.Models.Dtos.User;
using yummealAPI.Models.Entities;

namespace yummealAPI.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<UserProfile, UserProfileDto>()
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => (int?)src.Age)) // int -> int?
                .ForMember(dest => dest.Height, opt => opt.MapFrom(src => (int?)src.Height)) // int -> int?
                .ForMember(
                    dest => dest.CurrentWeight,
                    opt => opt.MapFrom(src => (decimal?)src.CurrentWeight)
                ) // int -> decimal?
                .ForMember(
                    dest => dest.DailyCalorieGoal,
                    opt => opt.MapFrom(src => (int?)src.DailyCaloricIntake)
                ) // DailyCaloricIntake -> DailyCalorieGoal
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.MapFrom(src => src.UpdatedAt ?? DateTime.UtcNow)
                );

            CreateMap<CreateUserProfileDto, UserProfile>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Age ?? 0)) // int? -> int
                .ForMember(dest => dest.Height, opt => opt.MapFrom(src => src.Height ?? 0)) // int? -> int
                .ForMember(
                    dest => dest.CurrentWeight,
                    opt => opt.MapFrom(src => (int)(src.CurrentWeight ?? 0))
                ) // decimal? -> int
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<UpdateUserProfileDto, UserProfile>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // UserPreferences mappings - różne nazwy!
            CreateMap<UserPreferences, UserPreferencesDto>()
                .ForMember(
                    dest => dest.PreferredCuisines,
                    opt => opt.MapFrom(src => src.FavoriteCuisines)
                ) // FavoriteCuisines -> PreferredCuisines
                .ForMember(dest => dest.MealFrequency, opt => opt.MapFrom(src => src.MelsPerDay)) // MelsPerDay -> MealFrequency
                .ForMember(
                    dest => dest.PreferredDifficulty,
                    opt => opt.MapFrom(src => src.DifficultyLevel)
                ) // DifficultyLevel -> PreferredDifficulty
                .ForMember(
                    dest => dest.AverageBudget,
                    opt => opt.MapFrom(src => (int)src.AverageBudget)
                ) // decimal -> int
                .ForMember(
                    dest => dest.UpdatedAt,
                    opt => opt.MapFrom(src => src.UpdatedAt ?? DateTime.UtcNow)
                );

            CreateMap<UserPreferencesDto, UserPreferences>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserProfileId, opt => opt.Ignore())
                .ForMember(dest => dest.UserProfile, opt => opt.Ignore())
                .ForMember(
                    dest => dest.FavoriteCuisines,
                    opt => opt.MapFrom(src => src.PreferredCuisines)
                ) // PreferredCuisines -> FavoriteCuisines
                .ForMember(dest => dest.MelsPerDay, opt => opt.MapFrom(src => src.MealFrequency)) // MealFrequency -> MelsPerDay
                .ForMember(
                    dest => dest.DifficultyLevel,
                    opt => opt.MapFrom(src => src.PreferredDifficulty)
                ) // PreferredDifficulty -> DifficultyLevel
                .ForMember(
                    dest => dest.AverageBudget,
                    opt => opt.MapFrom(src => (decimal)src.AverageBudget)
                ) // int -> decimal
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}
