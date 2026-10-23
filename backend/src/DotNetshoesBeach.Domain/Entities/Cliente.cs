using DotNetshoesBeach.Domain.Common;
using DotNetshoesBeach.Domain.Enums;

namespace DotNetshoesBeach.Domain.Entities;

public class Cliente : BaseEntity
{
    public string Nome { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string Telefone { get; private set; } = string.Empty;
    public string SenhaHash { get; private set; } = null!;
    public Papel Papel { get; private set; }

    protected Cliente() { }

    public Cliente(string nome, string email, string telefone, string senhaHash, Papel papel = Papel.Cliente)
    {
        if (string.IsNullOrWhiteSpace(senhaHash))
            throw new ArgumentException("O hash da senha é obrigatório.", nameof(senhaHash));

        if (!Enum.IsDefined(papel))
            throw new ArgumentException("Papel inválido.", nameof(papel));

        DefinirDadosCadastrais(nome, email, telefone);
        SenhaHash = senhaHash;
        Papel = papel;
    }

    public bool EhAdmin => Papel == Papel.Admin;

    public void Atualizar(string nome, string email, string telefone) =>
        DefinirDadosCadastrais(nome, email, telefone);

    public void AlterarSenha(string novoHash)
    {
        if (string.IsNullOrWhiteSpace(novoHash))
            throw new ArgumentException("O hash da senha é obrigatório.", nameof(novoHash));

        SenhaHash = novoHash;
    }

    public void DefinirPapel(Papel papel)
    {
        if (!Enum.IsDefined(papel))
            throw new ArgumentException("Papel inválido.", nameof(papel));

        Papel = papel;
    }

    private void DefinirDadosCadastrais(string nome, string email, string telefone)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome do cliente é obrigatório.", nameof(nome));

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new ArgumentException("Um e-mail válido é obrigatório.", nameof(email));

        Nome = nome.Trim();
        Email = email.Trim().ToLowerInvariant();
        Telefone = telefone?.Trim() ?? string.Empty;
    }
}
