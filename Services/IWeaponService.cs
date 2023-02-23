using DotNetRPG.DTO.Weapon;

namespace DotNetRPG.Services
{
    public interface IWeaponService
    {
        Task<ServiceResponse<GetCharacterDTO>> AddWeapon(AddWeaponDTO newWeapon);
    }
}
