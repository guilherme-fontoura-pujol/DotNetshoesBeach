using DotNetshoesBeach.Domain.Common;

namespace DotNetshoesBeach.Domain.Entities;

public class Cliente : BaseEntity
{
    public string Nome { get; private set; }
    public string Email { get; private set; }
    public string Telefone { get; private set; }
    public string SenhaHash { get; private set; }

    // Construtor sem parâmetros exigido por ferramentas de persistência (EF Core)
    protected Cliente() { }

    public Cliente(string nome, string email, string telefone, string senhaHash)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome do cliente é obrigatório.", nameof(nome));

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new ArgumentException("Um e-mail válido é obrigatório.", nameof(email));

        if (string.IsNullOrWhiteSpace(senhaHash))
            throw new ArgumentException("O hash da senha é obrigatório.", nameof(senhaHash));

        Nome = nome.Trim();
        Email = email.Trim().ToLowerInvariant();
        Telefone = telefone?.Trim() ?? string.Empty;
        SenhaHash = senhaHash;
    }
}