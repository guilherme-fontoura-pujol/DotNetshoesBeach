using DotNetshoesBeach.Application.Common;
using DotNetshoesBeach.Application.DTOs;
using DotNetshoesBeach.Application.Exceptions;
using DotNetshoesBeach.Application.Interfaces;

namespace DotNetshoesBeach.Application.Services;

public class AuthService : IAuthService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(IClienteRepository clienteRepository, IPasswordHasher passwordHasher, ITokenService tokenService)
    {
        _clienteRepository = clienteRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
    {
        const string mensagem = "E-mail ou senha inválidos.";

        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrEmpty(dto.Senha))
            throw new NaoAutenticadoException(mensagem);

        var usuario = await _clienteRepository.ObterPorEmailAsync(dto.Email, cancellationToken);

        // Mesma mensagem para e-mail inexistente e senha errada (não revela quais e-mails existem).
        if (usuario is null || !_passwordHasher.VerifyPassword(dto.Senha, usuario.SenhaHash))
            throw new NaoAutenticadoException(mensagem);

        var token = _tokenService.GerarToken(usuario);
        return new LoginResponseDto(token.Token, token.ExpiraEmUtc, usuario.ParaDto());
    }
}
