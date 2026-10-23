using DotNetshoesBeach.Domain.Common;
using DotNetshoesBeach.Domain.Enums;

namespace DotNetshoesBeach.Domain.Entities;

public class Quadra : BaseEntity
{
    public string Nome { get; private set; } = null!;
    public string Descricao { get; private set; } = string.Empty;
    public decimal ValorHora { get; private set; }
    public StatusQuadra Status { get; private set; }
    public string? MotivoManutencao { get; private set; }

    protected Quadra() { }

    public Quadra(string nome, string? descricao, decimal valorHora)
    {
        Atualizar(nome, descricao, valorHora);
        Status = StatusQuadra.Disponivel;
    }

    public bool EstaDisponivel => Status == StatusQuadra.Disponivel;

    public void Atualizar(string nome, string? descricao, decimal valorHora)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome da quadra é obrigatório.", nameof(nome));

        // Regra de negócio: o valor por hora de uma quadra deve ser maior que zero.
        if (valorHora <= 0)
            throw new ArgumentException("O valor por hora da quadra deve ser maior que zero.", nameof(valorHora));

        if (valorHora > 100_000m)
            throw new ArgumentException("O valor por hora informado é alto demais.", nameof(valorHora));

        Nome = nome.Trim();
        Descricao = descricao?.Trim() ?? string.Empty;
        ValorHora = Math.Round(valorHora, 2, MidpointRounding.AwayFromZero);
    }

    public void ColocarEmManutencao(string? motivo)
    {
        Status = StatusQuadra.EmManutencao;
        MotivoManutencao = string.IsNullOrWhiteSpace(motivo) ? null : motivo.Trim();
    }

    public void Disponibilizar()
    {
        Status = StatusQuadra.Disponivel;
        MotivoManutencao = null;
    }
}
