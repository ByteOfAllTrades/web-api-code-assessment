// /////////////////////////////////////////////////////////////////////////////
// YOU CAN FREELY MODIFY THE CODE BELOW IN ORDER TO COMPLETE THE TASK
// /////////////////////////////////////////////////////////////////////////////
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayerWebApi.Data;
using PlayerWebApi.Data.Entities;

namespace PlayerWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayerController : ControllerBase
{
    private readonly PlayerDbContext _dbContext;

    public PlayerController(PlayerDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        try
        {
            List<Player> players = await _dbContext.Players.Include(x => x.PlayerSkills).ToListAsync<Player>();
            return (Ok(players));
        }
        catch(Exception e)
        {
            return BadRequest(e.Message);
        }
       
        //throw new NotImplementedException();

    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(int id)
    {
        try
        {
            var Results = _dbContext.Players
              .Include(x => x.PlayerSkills)
              .SingleOrDefaultAsync(x => x.Id == id);
            return (Ok(Results.Result));
        }
        catch(Exception e)
        {
            return BadRequest(e.Message);
        }
        
        //throw new NotImplementedException();
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync(string name, string position, IEnumerable<PlayerSkill> playerSkills)
    {
        
        try
        {
            foreach (PlayerSkill playerSkill in playerSkills)
            {
                if (playerSkill.Value > 99 || playerSkill.Value < 0 ) throw new Exception("Invalid value for player skill " + playerSkill.Skill +": "+playerSkill.Value);
            }
            if (name == null || name == "") throw new Exception("Invalid value for name: "+name);
            if (playerSkills.Count() <= 0) throw new Exception("PlayerSkills must have at least one entry.");
            if (position == "defender" || position == "midfielder" || position == "forward")
            {
                Player player = new Player();
                player.Name = name;
                player.Position = position;
                player.PlayerSkills = playerSkills;
                await _dbContext.Players.AddAsync(player);
                _dbContext.SaveChanges();
                int id = player.Id;
                var Results = await _dbContext.Players.FindAsync(id);
                return (Ok(Results));
            }
            else
            {
                throw new Exception("Invalid value for position: " + position);
            }
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }



        //throw new NotImplementedException();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsync(int id, string name = null, string position = null, IEnumerable<PlayerSkill> playerSkills = null)
    {
        try
        { 
            foreach (PlayerSkill playerSkill in playerSkills)
            {
                if (playerSkill.Value > 99 || playerSkill.Value < 0 ) throw new Exception("Invalid value for player skill " + playerSkill.Skill +": "+playerSkill.Value);
            }
            if (playerSkills.Count() <= 0) throw new Exception("PlayerSkills must have at least one entry.");
            if (position == "defender" || position == "midfielder" || position == "forward" || position == "null")
            {
                Player player = _dbContext.Players.Where(s => s.Id == id).First();
                if (name != null)
                {
                    player.Name = name;
                }
                if (position != null)
                {
                    player.Position = position;
                }
                if (playerSkills != null)
                {
                    player.PlayerSkills = playerSkills;
                }
                _dbContext.SaveChanges();
                var Results = await _dbContext.Players.FindAsync(id);
                return (Ok(Results));
            }
            else
            {
                throw new Exception("Invalid value for position: " + position);
            }
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }

        //throw new NotImplementedException();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        try
        {
            Player player = await _dbContext.Players.FindAsync(id);
            _dbContext.Players.Attach(player);
            _dbContext.Players.Remove(player);
            _dbContext.SaveChanges();
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
        //throw new NotImplementedException();
    }
}
