using DotNetshoesBeach.Application.DTOs;

namespace DotNetshoesBeach.Application.Services;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default);
}
