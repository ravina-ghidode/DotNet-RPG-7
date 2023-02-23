using DotNetRPG.DTO.Weapon;
using System.Security.Claims;

namespace DotNetRPG.Services
{
    public class WeaponService : IWeaponService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WeaponService(DataContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ServiceResponse<GetCharacterDTO>> AddWeapon(AddWeaponDTO newWeapon)
        {
            var response = new ServiceResponse<GetCharacterDTO>();
            try
            {
                var character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == newWeapon.CharacterId &&
                c.User!.Id == int.Parse(_httpContextAccessor.HttpContext!.User.
                FindFirstValue(ClaimTypes.NameIdentifier)
                ));

                if (character == null)
                {
                    response.Success = false;
                    response.Message = "Character not found";
                    return response;
                }
                var weapon = new Weapon
                {
                    Name = newWeapon.Name,
                    Damage = newWeapon.Damage,
                    Character = character
                };
                _context.Weapons.Add(weapon);
                await _context.SaveChangesAsync();
                response.Data = _mapper.Map<GetCharacterDTO>(character);

            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;

        }
    }
}
