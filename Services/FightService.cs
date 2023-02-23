using DotNetRPG.DTO.Fight;

namespace DotNetRPG.Services
{
    public class FightService : IFightService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public FightService(DataContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<FightResultDTO>> Fight(FightRequestDTO request)
        {
            var response = new ServiceResponse<FightResultDTO>
            {
                Data = new FightResultDTO()
            };
            try
            {
                var characters = await _context.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.Skills)
                    .Where(c => request.CharacterIds.Contains(c.Id)).ToListAsync();
                bool defeated = false;
                while (!defeated)
                {
                    foreach(var attacker in characters)
                    {
                        var opponents = characters.Where(c => c.Id != attacker.Id).ToList();
                        var opponent  = opponents[new Random().Next(opponents.Count)];

                        int damage = 0;
                        string attackeUsed = string.Empty;

                        bool useWeapon = new Random().Next(2) == 0;
                        if(useWeapon && attacker.Weapon is not null)
                        {
                            attackeUsed = attacker.Weapon.Name;
                            damage = DoWeaponAttack(attacker, opponent);
                        }
                        else if( !useWeapon ||attacker.Skills is not null)
                        {
                            var skill = attacker.Skills[new Random().Next(attacker.Skills.Count)];
                            attackeUsed=skill.Name;
                            damage= DoSkillAttack(attacker, opponent,skill);

                        }
                        else
                        {
                            response.Data.Log.Add($"{attacker.Name} was not able to attack");
                            continue;
                        }
                        response.Data.Log.Add($"{attacker.Name} attacks {opponent.Name} using {attackeUsed} with {(damage >= 0 ? damage : 0)}damage");

                        if(opponent.HitPoints <= 0)
                        {
                            defeated = true;
                            attacker.Victories++;
                            opponent.Defeats++;
                            response.Data.Log.Add($"{opponent.Name} has been defeated");
                            response.Data.Log.Add($"{attacker.Name} wins with {attacker.HitPoints} HP left");
                            break;
                        }
                    }
                }
                characters.ForEach(c =>
                {
                    c.Fights++;
                    c.HitPoints = 100;

                });

                await _context.SaveChangesAsync();

            }
            catch(Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;

            
        }

        public async Task<ServiceResponse<AttackResultDTO>> SkillAttack(SkillAttackDTO request)
        {
            var response = new ServiceResponse<AttackResultDTO>();
            try
            {
                var attacker = await _context.Characters.Include(c => c.Skills).
                    FirstOrDefaultAsync(c => c.Id == request.AttackerId);
                var opponent = await _context.Characters.
                    FirstOrDefaultAsync(c => c.Id == request.OpponentId);

                if (attacker is null || opponent is null || attacker.Skills is null)
                {
                    throw new Exception("Something fishy is going on here");
                }
                var skill = attacker.Skills.FirstOrDefault(s => s.Id == request.SkillId);
                if (skill is null)
                {
                    response.Success = false;
                    response.Message = $"{attacker.Name} doesnot know this skill";
                    return response;
                }
                int damage = DoSkillAttack(attacker, opponent, skill);
                if (opponent.HitPoints <= 0)
                {
                    response.Message = $"{opponent.Name} has been defeated";
                }
                await _context.SaveChangesAsync();
                response.Data = new AttackResultDTO
                {
                    Attacker = attacker.Name,
                    Opponent = attacker.Name,
                    AttackerHP = attacker.HitPoints,
                    OpponentHP = opponent.HitPoints,
                    Damage = damage,
                };

            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;

            }
            return response;

        }

        private static int DoSkillAttack(Character attacker, Character opponent, Skill skill)
        {
            int damage = skill.Damage + (new Random().Next(attacker.Intelligence));
            damage -= new Random().Next(opponent.Defeats);
            if (damage > 0)
            {
                opponent.HitPoints -= damage;
            }

            return damage;
        }

        public async Task<ServiceResponse<AttackResultDTO>> WeaponAttack(WeaponAttackDTO request)
        {
            var response = new ServiceResponse<AttackResultDTO>();
            try
            {
                var attacker = await _context.Characters.Include(c => c.Weapon).
                    FirstOrDefaultAsync(c => c.Id == request.AttackerId);
                var opponent = await _context.Characters.
                    FirstOrDefaultAsync(c => c.Id == request.OpponentId);

                if (attacker is null || opponent is null || attacker.Weapon is null)
                {
                    throw new Exception("Something fishy is going on here");
                }
                int damage = DoWeaponAttack(attacker, opponent);
                if (opponent.HitPoints <= 0)
                {
                    response.Message = $"{opponent.Name} has been defeated";
                }
                await _context.SaveChangesAsync();
                response.Data = new AttackResultDTO
                {
                    Attacker = attacker.Name,
                    Opponent = attacker.Name,
                    AttackerHP = attacker.HitPoints,
                    OpponentHP = opponent.HitPoints,
                    Damage = damage,
                };

            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;

            }
            return response;
        }

        private static int DoWeaponAttack(Character attacker, Character opponent)
        {
            if(attacker.Weapon is null)
            {
                throw new Exception("Attacker has no weapon");
            }
            int damage = attacker.Weapon.Damage + (new Random().Next(attacker.Strength));
            damage -= new Random().Next(opponent.Defeats);
            if (damage > 0)
            {
                opponent.HitPoints -= damage;
            }

            return damage;
        }

        public async Task<ServiceResponse<List<HighScoreDTO>>> GetHighScore()
        {
            var characters = await _context.Characters
                .Where(c => c.Fights > 0)
                .OrderByDescending(c => c.Victories)
                .ThenBy(c => c.Defeats)
                .ToListAsync();
            var response = new ServiceResponse<List<HighScoreDTO>>()
            {
                Data = characters.Select(c => _mapper.Map<HighScoreDTO>(c)).ToList(),
            };
            return response;
            
        }
    }
}
