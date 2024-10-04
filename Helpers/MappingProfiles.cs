using AutoMapper;
using TFT_API.Models.Unit;
using TFT_API.Models.Item;
using TFT_API.Models.User;
using TFT_API.Models.Trait;
using TFT_API.Models.UserGuides;
using TFT_API.Models.Votes;
using TFT_API.Models.Augments;
using TFT_API.Helpers;
using TFT_API.Models.FetchedTFTData;

namespace TFT_API.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<UserLoginRequest, PersistedUser>();
            CreateMap<AddUserRequest, PersistedUser>();
            CreateMap<PersistedUser, UserDto>();
            CreateMap<UserGuideRequest, UserGuide>()
                .ForMember(dest => dest.Traits, opt => opt.MapFrom<UserGuideResolver>())
                .ForMember(dest => dest.Hexes, opt => opt.MapFrom<HexResolver>())
                .ForMember(dest => dest.Augments, opt => opt.MapFrom<AugmentResolver>());
            CreateMap<VoteRequest, Vote>();
            CreateMap<CommentRequest, Comment>();
            CreateMap<PersistedItem, PersistedItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<PersistedUnit, PersistedUnit>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Skill, opt => opt.MapFrom(src => src.Skill))
                .ForMember(dest => dest.Traits, opt => opt.MapFrom(src => src.Traits));
            CreateMap<PersistedTrait, PersistedTrait>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Tiers, opt => opt.MapFrom(t => t.Tiers));
            CreateMap<PersistedAugment, PersistedAugment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<Skill, Skill>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<TraitTier, TraitTier>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
                
            CreateMap<Champion, PersistedUnit>()
                .ForMember(dest => dest.InGameKey, opt => opt.MapFrom(src => src.IngameKey))
                .ForMember(dest => dest.Health, opt => opt.MapFrom(src => src.Health.Select(h => (int)Math.Round((double)h)).ToList()))
                .ForMember(dest => dest.Health, opt => opt.MapFrom(src => src.Cost.FirstOrDefault()))
                .ForMember(dest => dest.Traits, opt => opt.MapFrom<TraitResolver>());
            CreateMap<Item, PersistedItem>()
                .ForMember(dest => dest.InGameKey, opt => opt.MapFrom(src => src.IngameKey))
                .ForMember(dest => dest.IsHidden, opt => opt.MapFrom(src => src.Tags.Contains("hidden")))
                .ForMember(dest => dest.Recipe, opt => opt.MapFrom(src => src.Compositions))
                .ForMember(dest => dest.IsComponent, opt => opt.MapFrom(src => src.IsFromItem))
                .ForMember(dest => dest.Desc, opt => opt.MapFrom(src => src.Desc.Replace("\u200b", "")));
            CreateMap<Augment, PersistedAugment>()
                .ForMember(dest => dest.InGameKey, opt => opt.MapFrom(src => src.IngameKey))
                .ForMember(dest => dest.IsHidden, opt => opt.MapFrom(src => src.IsHidden.GetValueOrDefault() || src.IngameKey.Contains("HR")));
            CreateMap<Trait, PersistedTrait>()
                .ForMember(dest => dest.InGameKey, opt => opt.MapFrom(src => src.IngameKey))
                .ForMember(dest => dest.Tiers, opt => opt.MapFrom(src => src.StageStyles))
                .ForMember(dest => dest.TierString, opt => opt.MapFrom(src => string.Join(" / ", src.Stats.Keys)));
            CreateMap<StageStyle, TraitTier>()
                .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Min))
                .ForMember(dest => dest.Rarity, opt => opt.MapFrom(src =>
                            src.Style == "bronze" ? 1 :
                            src.Style == "silver" ? 2 :
                            src.Style == "gold" ? 3 :
                            src.Style == "chromatic" ? 4 : (int?)null));             
        }
    }
}
