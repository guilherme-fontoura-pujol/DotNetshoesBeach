using DotNetshoesBeach.Application.DTOs;

namespace DotNetshoesBeach.Application.Services;

public interface IClienteService
{
    Task<ClienteResponseDto> CadastrarAsync(CadastrarClienteDto dto, CancellationToken cancellationToken = default);
}