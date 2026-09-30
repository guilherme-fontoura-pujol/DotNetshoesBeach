using DotNetshoesBeach.Domain.Common;

namespace DotNetshoesBeach.Domain.Entities;

public class Quadra : BaseEntity
{
    public string Nome { get; private set; }
    public bool Ativa { get; private set; }

    protected Quadra() { }

    public Quadra(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome da quadra é obrigatório.", nameof(nome));

        Nome = nome.Trim();
        Ativa = true;
    }

    public void Desativar() => Ativa = false;
    public void Ativar() => Ativa = true;
}