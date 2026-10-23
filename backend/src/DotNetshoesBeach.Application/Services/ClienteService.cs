using DotNetshoesBeach.Application.Common;
using DotNetshoesBeach.Application.DTOs;
using DotNetshoesBeach.Application.Exceptions;
using DotNetshoesBeach.Application.Interfaces;
using DotNetshoesBeach.Domain.Entities;
using DotNetshoesBeach.Domain.Enums;

namespace DotNetshoesBeach.Application.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IReservaRepository _reservaRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _clock;

    public ClienteService(
        IClienteRepository clienteRepository,
        IReservaRepository reservaRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork,
        TimeProvider clock)
    {
        _clienteRepository = clienteRepository;
        _reservaRepository = reservaRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public Task<ClienteResponseDto> CadastrarAsync(CadastrarClienteDto dto, CancellationToken cancellationToken = default) =>
        CriarInternoAsync(dto.Nome, dto.Email, dto.Telefone, dto.Senha, Papel.Cliente, cancellationToken);

    public Task<ClienteResponseDto> CriarAsync(AdminCriarClienteDto dto, CancellationToken cancellationToken = default) =>
        CriarInternoAsync(dto.Nome, dto.Email, dto.Telefone, dto.Senha, dto.Papel, cancellationToken);

    public async Task<IReadOnlyList<ClienteResponseDto>> ListarAsync(CancellationToken cancellationToken = default)
    {
        var clientes = await _clienteRepository.ListarAsync(cancellationToken);
        return clientes.Select(c => c.ParaDto()).ToList();
    }

    public async Task<ClienteResponseDto> ObterAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cliente = await ObterOuFalharAsync(id, cancellationToken);
        return cliente.ParaDto();
    }

    public async Task<ClienteResponseDto> AtualizarAsync(
        Guid id, AtualizarClienteDto dto, UsuarioAtual solicitante, CancellationToken cancellationToken = default)
    {
        var cliente = await ObterOuFalharAsync(id, cancellationToken);

        if (dto.Papel.HasValue && dto.Papel.Value != cliente.Papel && cliente.Id == solicitante.Id)
            throw new RegraDeNegocioException("Você não pode alterar o seu próprio papel.");

        // Valida antes de alterar a entidade (e antes de consultar o banco, quando possível).
        cliente.Atualizar(dto.Nome, dto.Email, dto.Telefone);

        if (await _clienteRepository.ExisteEmailAsync(cliente.Email, ignorarId: cliente.Id, cancellationToken))
            throw new ConflitoException("Já existe um cliente cadastrado com o e-mail informado.");

        if (dto.Papel.HasValue)
            cliente.DefinirPapel(dto.Papel.Value);

        if (!string.IsNullOrWhiteSpace(dto.NovaSenha))
        {
            SenhaPolicy.Validar(dto.NovaSenha);
            cliente.AlterarSenha(_passwordHasher.HashPassword(dto.NovaSenha));
        }

        await _unitOfWork.SalvarAsync(cancellationToken);
        return cliente.ParaDto();
    }

    public async Task ExcluirAsync(Guid id, UsuarioAtual solicitante, CancellationToken cancellationToken = default)
    {
        if (id == solicitante.Id)
            throw new RegraDeNegocioException("Você não pode excluir a sua própria conta.");

        var cliente = await ObterOuFalharAsync(id, cancellationToken);

        var agora = _clock.GetUtcNow().UtcDateTime;
        if (await _reservaRepository.ClienteTemReservasAtivasAsync(id, agora, cancellationToken))
            throw new RegraDeNegocioException(
                "O cliente possui reservas ativas ou futuras. Cancele-as antes de excluir a conta.");

        // O histórico de reservas antigas/canceladas é removido junto (cascade no banco).
        _clienteRepository.Remover(cliente);
        await _unitOfWork.SalvarAsync(cancellationToken);
    }

    private async Task<ClienteResponseDto> CriarInternoAsync(
        string nome, string email, string telefone, string senha, Papel papel, CancellationToken cancellationToken)
    {
        SenhaPolicy.Validar(senha);

        // 1. Regra de Aplicação: unicidade de e-mail
        if (await _clienteRepository.ExisteEmailAsync(email ?? string.Empty, null, cancellationToken))
            throw new ConflitoException("Já existe um cliente cadastrado com o e-mail informado.");

        // 2. Segurança: hashear a senha
        var senhaHash = _passwordHasher.HashPassword(senha);

        // 3. Domínio: entidade rica valida os invariantes
        var novoCliente = new Cliente(nome, email!, telefone, senhaHash, papel);

        // 4. Persistência
        await _clienteRepository.AdicionarAsync(novoCliente, cancellationToken);
        await _unitOfWork.SalvarAsync(cancellationToken);

        return novoCliente.ParaDto();
    }

    private async Task<Cliente> ObterOuFalharAsync(Guid id, CancellationToken cancellationToken) =>
        await _clienteRepository.ObterPorIdAsync(id, cancellationToken)
        ?? throw new NaoEncontradoException("Cliente não encontrado.");
}
