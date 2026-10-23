using DotNetshoesBeach.Application.Common;
using DotNetshoesBeach.Application.DTOs;

namespace DotNetshoesBeach.Application.Services;

public interface IClienteService
{
    /// <summary>Auto-cadastro público: sempre cria uma conta com papel Cliente.</summary>
    Task<ClienteResponseDto> CadastrarAsync(CadastrarClienteDto dto, CancellationToken cancellationToken = default);

    // --- Operações de admin ---
    Task<ClienteResponseDto> CriarAsync(AdminCriarClienteDto dto, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ClienteResponseDto>> ListarAsync(CancellationToken cancellationToken = default);
    Task<ClienteResponseDto> ObterAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ClienteResponseDto> AtualizarAsync(Guid id, AtualizarClienteDto dto, UsuarioAtual solicitante, CancellationToken cancellationToken = default);
    Task ExcluirAsync(Guid id, UsuarioAtual solicitante, CancellationToken cancellationToken = default);
}
