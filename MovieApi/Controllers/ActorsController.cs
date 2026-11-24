using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApi.Data;
using MovieApi.DTOs;
using MovieApi.DTOs.Mappings;

namespace MovieApi.Controllers;

[Route("api/actors")]
[ApiController]
public class ActorsController : ControllerBase
{
    private readonly MovieDbContext _context;

    public ActorsController(MovieDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ActorDto>>> GetActors()
    {
        var actors = await _context.Actors
            .AsNoTracking()
            .OrderBy(actor => actor.Name)
            .ToListAsync();

        return actors.Select(actor => actor.ToDto()).ToList();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ActorDto>> GetActor(int id)
    {
        var actor = await _context.Actors
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id);

        return actor is null ? NotFound() : actor.ToDto();
    }

    [HttpPost]
    public async Task<ActionResult<ActorDto>> CreateActor(ActorDto actorDto)
    {
        var actor = actorDto.ToEntity();

        _context.Actors.Add(actor);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetActor), new { id = actor.Id }, actor.ToDto());
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateActor(int id, ActorDto actorDto)
    {
        if (id != actorDto.Id)
        {
            return BadRequest();
        }

        var actor = await _context.Actors.FindAsync(id);
        if (actor is null)
        {
            return NotFound();
        }

        actorDto.UpdateEntity(actor);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}