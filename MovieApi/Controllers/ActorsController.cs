using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
    public async Task<Ok<IReadOnlyList<ActorResponse>>> GetActors(CancellationToken ct = default)
    {
        var actors = await _unitOfWork.Actors.GetAllAsync(ct);

        return TypedResults.Ok<IReadOnlyList<ActorResponse>>(
            [.. actors.Select(actor => actor.ToResponse())]);
    }

    [HttpGet("{id:int}", Name = nameof(GetActor))]
    public async Task<Results<Ok<ActorResponse>, NotFound>> GetActor(int id, CancellationToken ct = default)
    {
        var actor = await _unitOfWork.Actors.GetByIdAsync(id, ct);

        return actor is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(actor.ToResponse());
    }

    [HttpPost]
    public async Task<CreatedAtRoute<ActorResponse>> CreateActor(
        CreateActorRequest request,
        CancellationToken ct = default)
    {
        var actor = request.ToEntity();

        _unitOfWork.Actors.Add(actor);
        await _unitOfWork.SaveChangesAsync(ct);

        return TypedResults.CreatedAtRoute(actor.ToResponse(), nameof(GetActor), new { id = actor.Id });
    }

    [HttpPut("{id:int}")]
    public async Task<Results<NoContent, NotFound>> UpdateActor(
        int id,
        UpdateActorRequest request,
        CancellationToken ct = default)
    {
        var actor = await _unitOfWork.Actors.FindByIdAsync(id, ct);
        if (actor is null)
        {
            return TypedResults.NotFound();
        }

        request.UpdateEntity(actor);
        await _unitOfWork.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }
}