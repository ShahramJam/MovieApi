using Microsoft.AspNetCore.Mvc;
using MovieApi.Data;
using MovieApi.DTOs.Actors;
using MovieApi.DTOs.Mappings;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MovieApi.Controllers;

[Route("api/actors")]
[ApiController]
public class ActorsController(IUnitOfWork unitOfWork) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ActorResponse>>> GetActors(CancellationToken ct = default)
    {
        var actors = await _unitOfWork.Actors.GetAllAsync(ct);

        return actors.Select(actor => actor.ToResponse()).ToList();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ActorResponse>> GetActor(int id, CancellationToken ct = default)
    {
        var actor = await _unitOfWork.Actors.GetByIdAsync(id, ct);

        return actor is null ? NotFound() : actor.ToResponse();
    }

    [HttpPost]
    public async Task<ActionResult<ActorResponse>> CreateActor(
        CreateActorRequest request,
        CancellationToken ct = default)
    {
        var actor = request.ToEntity();

        _unitOfWork.Actors.Add(actor);
        await _unitOfWork.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetActor), new { id = actor.Id }, actor.ToResponse());
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateActor(
        int id,
        UpdateActorRequest request,
        CancellationToken ct = default)
    {
        var actor = await _unitOfWork.Actors.FindByIdAsync(id, ct);
        if (actor is null)
        {
            return NotFound();
        }

        request.UpdateEntity(actor);
        await _unitOfWork.SaveChangesAsync(ct);

        return NoContent();
    }
}