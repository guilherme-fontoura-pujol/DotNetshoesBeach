using DotNetshoesBeach.Api.Support;
using DotNetshoesBeach.Application.DTOs;
using DotNetshoesBeach.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNetshoesBeach.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IClienteService _clienteService;

    public AuthController(IAuthService authService, IClienteService clienteService)
    {
        _authService = authService;
        _clienteService = clienteService;
    }

    /// <summary>Login de admin ou cliente. Retorna o token JWT para o header Authorization: Bearer.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto dto, CancellationToken ct) =>
        Ok(await _authService.LoginAsync(dto, ct));

    /// <summary>Auto-cadastro público de cliente (sempre com papel Cliente).</summary>
    [HttpPost("registrar")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ClienteResponseDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Registrar([FromBody] CadastrarClienteDto dto, CancellationToken ct)
    {
        var criado = await _clienteService.CadastrarAsync(dto, ct);
        return StatusCode(StatusCodes.Status201Created, criado);
    }

    /// <summary>Dados do usuário logado.</summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ClienteResponseDto>> Me(CancellationToken ct) =>
        Ok(await _clienteService.ObterAsync(User.ObterUsuarioAtual().Id, ct));
}
