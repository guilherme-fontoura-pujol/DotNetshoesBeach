using DotNetshoesBeach.Application.DTOs;
using DotNetshoesBeach.Application.Interfaces;
using DotNetshoesBeach.Domain.Entities;

namespace DotNetshoesBeach.Application.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IPasswordHasher _passwordHasher;

    public ClienteService(IClienteRepository clienteRepository, IPasswordHasher passwordHasher)
    {
        _clienteRepository = clienteRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<ClienteResponseDto> CadastrarAsync(CadastrarClienteDto dto, CancellationToken cancellationToken = default)
    {
        // 1. Regra de Aplicação: verificar unicidade de e-mail
        var emailExiste = await _clienteRepository.ExisteEmailAsync(dto.Email, cancellationToken);
        if (emailExiste)
        {
            throw new InvalidOperationException("Já existe um cliente cadastrado com o e-mail informado.");
        }

        // 2. Segurança: hashear a senha
        var senhaHash = _passwordHasher.HashPassword(dto.Senha);

        // 3. Domínio: instanciar entidade rica (dispara validações invariantes do construtor)
        var novoCliente = new Cliente(dto.Nome, dto.Email, dto.Telefone, senhaHash);

        // 4. Persistência através de abstração
        await _clienteRepository.AdicionarAsync(novoCliente, cancellationToken);

        // 5. Retornar DTO de resposta sem expor senha ou detalhes internos
        return new ClienteResponseDto(
            novoCliente.Id,
            novoCliente.Nome,
            novoCliente.Email,
            novoCliente.Telefone
        );
    }
}