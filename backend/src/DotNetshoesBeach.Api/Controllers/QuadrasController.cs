using DotNetshoesBeach.Api.Support;
using DotNetshoesBeach.Application.DTOs;
using DotNetshoesBeach.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNetshoesBeach.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuadrasController : ControllerBase
{
    private readonly IQuadraService _quadraService;

    public QuadrasController(IQuadraService quadraService)
    {
        _quadraService = quadraService;
    }

    // ---------- Admin e cliente ----------

    /// <summary>Lista todas as quadras com o status (Disponivel / EmManutencao).</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<QuadraResponseDto>>> Listar(CancellationToken ct) =>
        Ok(await _quadraService.ListarAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<QuadraResponseDto>> Obter(Guid id, CancellationToken ct) =>
        Ok(await _quadraService.ObterAsync(id, ct));

    /// <summary>Horários já reservados no período (de/ate em ISO 8601). Usado para montar a agenda de horários livres.</summary>
    [HttpGet("{id:guid}/ocupados")]
    public async Task<ActionResult<IReadOnlyList<PeriodoOcupadoDto>>> Ocupados(
        Guid id, [FromQuery] DateTimeOffset de, [FromQuery] DateTimeOffset ate, CancellationToken ct) =>
        Ok(await _quadraService.ListarOcupadosAsync(id, de, ate, ct));

    // ---------- Somente admin ----------

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(QuadraResponseDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Criar([FromBody] SalvarQuadraDto dto, CancellationToken ct)
    {
        var criada = await _quadraService.CriarAsync(dto, ct);
        return CreatedAtAction(nameof(Obter), new { id = criada.Id }, criada);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<QuadraResponseDto>> Atualizar(Guid id, [FromBody] SalvarQuadraDto dto, CancellationToken ct) =>
        Ok(await _quadraService.AtualizarAsync(id, dto, ct));

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken ct)
    {
        await _quadraService.ExcluirAsync(id, ct);
        return NoContent();
    }

    /// <summary>Coloca a quadra em manutenção. Se houver reservas ativas, exige "cancelarReservas": true.</summary>
    [HttpPost("{id:guid}/manutencao")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<QuadraResponseDto>> ColocarEmManutencao(
        Guid id, [FromBody] ColocarEmManutencaoDto dto, CancellationToken ct) =>
        Ok(await _quadraService.ColocarEmManutencaoAsync(id, dto, ct));

    /// <summary>Tira a quadra de manutenção e volta a aceitar reservas.</summary>
    [HttpPost("{id:guid}/disponibilizar")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<QuadraResponseDto>> Disponibilizar(Guid id, CancellationToken ct) =>
        Ok(await _quadraService.DisponibilizarAsync(id, ct));
}
