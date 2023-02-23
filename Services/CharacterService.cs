

using System.Linq;
using System.Security.Claims;

namespace DotNetRPG.Services
{
    public class CharacterService : ICharacterService
    {
       
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CharacterService(IMapper mapper,DataContext context,IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext!.User
            .FindFirstValue(ClaimTypes.NameIdentifier)!);
        public async Task<ServiceResponse<List<GetCharacterDTO>>> AddCharacter(AddCharacterDTO newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDTO>>();
            var character = _mapper.Map<Character>(newCharacter);

            character.User = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());
           
             _context.Characters.Add(character);
            await _context.SaveChangesAsync();

            serviceResponse.Data = 
                await _context.Characters
                .Where(c => c.Id == GetUserId()).
                
                Select(c => _mapper.Map<GetCharacterDTO>(c)).ToListAsync();
            return serviceResponse;
        }

        public  async Task<ServiceResponse<List<GetCharacterDTO>>> DeleteCharacter(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDTO>>();
            try
            {
                var character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id && c.User!.Id == GetUserId());
                if (character == null)
                {
                    throw new Exception($"character with id '{id}' is not found");
                }
                _context.Characters.Remove(character);
                await _context.SaveChangesAsync();
                serviceResponse.Data = await _context.Characters.Where(c => c.User!.Id == GetUserId())
                    .Select(c => _mapper.Map<GetCharacterDTO>(c)).ToListAsync();


            }
            catch(Exception e)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = e.Message;

            }
            return serviceResponse;
        }

        public async  Task<ServiceResponse<List<GetCharacterDTO>>> GetAllCharacter()
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDTO>>();
            var dbCharacters = await _context.Characters
                .Include(c => c.Weapon)
                .Include(c => c.Skills)
                .Where(c=> c.User!.Id == GetUserId())
                .ToListAsync();
            serviceResponse.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDTO>(c)).ToList();
            return serviceResponse;
        }

       

        public async Task<ServiceResponse<GetCharacterDTO>> GetCharacterById(int id)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDTO>();
            var dbCharacter =  await _context.Characters
                .Include(c => c.Weapon)
                .Include(c => c.Skills)
                .FirstOrDefaultAsync(c => c.Id == id && c.User!.Id == GetUserId());
            serviceResponse.Data = _mapper.Map<GetCharacterDTO>(dbCharacter);
            return serviceResponse;




        }

        public async Task<ServiceResponse<GetCharacterDTO>> UpdateCharacter(UpdateCharacterDTO updatedCharacter)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDTO>();
            try
            {
                var character =
                    await _context.Characters
                    .Include(c => c.User).FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id);
                if(character == null || character.User!.Id != GetUserId())
                {
                    throw new Exception($"character with id '{updatedCharacter.Id}' is not found");
                }
                _mapper.Map(updatedCharacter, character);
                character.Name = updatedCharacter.Name;
                character.Defense = updatedCharacter.Defense;
                character.Strength = updatedCharacter.Strength;
                character.HitPoints = updatedCharacter.HitPoints;
                character.Class = updatedCharacter.Class;
                character.Intelligence = updatedCharacter.Intelligence;
                await _context.SaveChangesAsync();

                serviceResponse.Data = _mapper.Map<GetCharacterDTO>(character);

            }
            catch (Exception e)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = e.Message;

            }
            
            return serviceResponse;


        }

        public async Task<ServiceResponse<GetCharacterDTO>> AddCharacterSkill(AddCharacterSkillDTO newCharacterSkill)
        {
           var response = new ServiceResponse<GetCharacterDTO>();
            try
            {
                var character = await _context.Characters
                    .Include(c => c.Weapon)
                    .Include(c=> c.Skills)
                    .FirstOrDefaultAsync(c => c.Id == newCharacterSkill.CharacterId
                && c.User!.Id == GetUserId());
                if(character == null)
                {
                    response.Success = false;
                    response.Message = "Character Not found";
                    return response;
                }

                var skill = await _context.Skills.FirstOrDefaultAsync(s => s.Id == newCharacterSkill.SkillId);
                if (skill == null)
                {
                    response.Success = false;
                    response.Message = "skill Not found";
                    return response;
                }
                character.Skills.Add(skill);
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
