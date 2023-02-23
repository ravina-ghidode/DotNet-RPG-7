using DotNetRPG.DTO.Weapon;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotNetRPG.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WeaponController : ControllerBase
    {
        private readonly IWeaponService _weaponService;

        public WeaponController(IWeaponService weaponService)
        {
            _weaponService = weaponService;
        }
        [HttpPost]
      public async  Task<ActionResult<ServiceResponse<GetCharacterDTO>>> AddWeapon(AddWeaponDTO newWeapon)
        {
            var response =  await _weaponService.AddWeapon(newWeapon);
            return Ok(response);
        }
    }
}
