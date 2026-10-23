using DotNetshoesBeach.Api.Support;
using DotNetshoesBeach.Application.DTOs;
using DotNetshoesBeach.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNetshoesBeach.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReservasController : ControllerBase
{
    private readonly IReservaService _reservaService;

    public ReservasController(IReservaService reservaService)
    {
        _reservaService = reservaService;
    }

    /// <summary>
    /// Cria uma reserva. Cliente reserva para si; admin pode informar "clienteId" para reservar em nome de alguém.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ReservaResponseDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Criar([FromBody] CriarReservaDto dto, CancellationToken ct)
    {
        var criada = await _reservaService.CriarAsync(dto, User.ObterUsuarioAtual(), ct);
        return StatusCode(StatusCodes.Status201Created, criada);
    }

    /// <summary>Reservas do usuário logado (painel do cliente).</summary>
    [HttpGet("minhas")]
    public async Task<ActionResult<IReadOnlyList<ReservaResponseDto>>> Minhas(CancellationToken ct) =>
        Ok(await _reservaService.ListarDoUsuarioAsync(User.ObterUsuarioAtual(), ct));

    /// <summary>Reservas paginadas, com filtros opcionais (quadraId, clienteId, de, ate, status, pagina, tamanhoPagina) — somente admin.</summary>
    [HttpGet]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<PaginaDto<ReservaResponseDto>>> Listar([FromQuery] FiltroReservaDto filtro, CancellationToken ct) =>
        Ok(await _reservaService.ListarAsync(filtro, ct));

    /// <summary>Cancela uma reserva. Cliente só cancela as próprias (antes de começar); admin cancela qualquer uma.</summary>
    [HttpPost("{id:guid}/cancelar")]
    public async Task<ActionResult<ReservaResponseDto>> Cancelar(Guid id, CancellationToken ct) =>
        Ok(await _reservaService.CancelarAsync(id, User.ObterUsuarioAtual(), ct));
}
