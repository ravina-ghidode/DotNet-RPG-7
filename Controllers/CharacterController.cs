
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DotNetRPG.Controllers
{
    [Authorize]

    [Route("api/[controller]")]
    [ApiController]
    public class CharacterController : ControllerBase
    {
        private static List<Character> characters = new List<Character>
        {
            new Character(),
            new Character { Id=1,Name = "Sam"}
        };
        private readonly ICharacterService _characterService;

        public CharacterController(ICharacterService characterService)
        {
            _characterService = characterService;
        }


       
        [HttpGet("getAll")]

        public async Task<ActionResult<ServiceResponse <List<GetCharacterDTO>>>> Get()
        {
           
            return Ok(await _characterService.GetAllCharacter());
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDTO>>>> GetSingle(int id)
        {
            return Ok(await _characterService.GetCharacterById(id));
        }
        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDTO>>>> AddCharacter(AddCharacterDTO newCharacter)
        {
            
            return Ok(await _characterService.AddCharacter(newCharacter));

        }
        [HttpPut]
        public async Task<ActionResult<ServiceResponse<GetCharacterDTO>>> UpdateCharacter(UpdateCharacterDTO updatedCharacter)
        {
            var response = await _characterService.UpdateCharacter(updatedCharacter);
            if(response.Data is  null)
            {
                return NotFound(response);
            }
            return Ok(response);

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDTO>>>> DeleteCharacter(int id)
        {
            var response = await _characterService.DeleteCharacter(id);
            if(response.Data is null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
        [HttpPost("Skill")]
        public async Task<ActionResult<ServiceResponse<GetCharacterDTO>>> AddCharacterSkill(
            AddCharacterSkillDTO newCharacterSkill)
        {
            var response = await _characterService.AddCharacterSkill(newCharacterSkill);
            return Ok(response);
        }

    }
}
