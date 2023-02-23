using DotNetRPG.DTO.Fight;
using DotNetRPG.DTO.Skill;
using DotNetRPG.DTO.Weapon;

namespace DotNetRPG
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Character,GetCharacterDTO>();
            CreateMap<AddCharacterDTO, Character>();
            CreateMap<UpdateCharacterDTO, Character>();
            CreateMap<Weapon, GetWeaponDTO>();
            CreateMap<Skill, GetSkillDTO>();
            CreateMap<Character, HighScoreDTO>();
        }
    }
}
