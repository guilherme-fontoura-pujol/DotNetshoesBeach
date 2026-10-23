using DotNetshoesBeach.Api.Support;
using DotNetshoesBeach.Application.DTOs;
using DotNetshoesBeach.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNetshoesBeach.Api.Controllers;

/// <summary>Indicadores gerenciais — somente admin.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class RelatoriosController : ControllerBase
{
    private readonly IRelatorioService _relatorioService;

    public RelatoriosController(IRelatorioService relatorioService)
    {
        _relatorioService = relatorioService;
    }

    /// <summary>
    /// Por quadra: quantidade de reservas confirmadas, valor total e valor médio no período [de, ate).
    /// Ordenado da quadra mais reservada para a menos reservada. Ex.: de=2026-10-01T00:00:00-03:00&amp;ate=2026-11-01T00:00:00-03:00
    /// </summary>
    [HttpGet("quadras")]
    public async Task<ActionResult<IReadOnlyList<ResumoQuadraDto>>> PorQuadra(
        [FromQuery] DateTimeOffset de, [FromQuery] DateTimeOffset ate, CancellationToken ct) =>
        Ok(await _relatorioService.ResumoPorQuadraAsync(de, ate, ct));
}
