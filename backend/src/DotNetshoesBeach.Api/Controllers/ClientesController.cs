using DotNetshoesBeach.Api.Support;
using DotNetshoesBeach.Application.DTOs;
using DotNetshoesBeach.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNetshoesBeach.Api.Controllers;

/// <summary>Gestão de contas de clientes — somente admin.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ClienteResponseDto>>> Listar(CancellationToken ct) =>
        Ok(await _clienteService.ListarAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ClienteResponseDto>> Obter(Guid id, CancellationToken ct) =>
        Ok(await _clienteService.ObterAsync(id, ct));

    [HttpPost]
    [ProducesResponseType(typeof(ClienteResponseDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Criar([FromBody] AdminCriarClienteDto dto, CancellationToken ct)
    {
        var criado = await _clienteService.CriarAsync(dto, ct);
        return CreatedAtAction(nameof(Obter), new { id = criado.Id }, criado);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ClienteResponseDto>> Atualizar(Guid id, [FromBody] AtualizarClienteDto dto, CancellationToken ct) =>
        Ok(await _clienteService.AtualizarAsync(id, dto, User.ObterUsuarioAtual(), ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken ct)
    {
        await _clienteService.ExcluirAsync(id, User.ObterUsuarioAtual(), ct);
        return NoContent();
    }
}
