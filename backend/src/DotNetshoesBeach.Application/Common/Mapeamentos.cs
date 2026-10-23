using DotNetshoesBeach.Application.DTOs;
using DotNetshoesBeach.Domain.Entities;

namespace DotNetshoesBeach.Application.Common;

internal static class Mapeamentos
{
    public static ClienteResponseDto ParaDto(this Cliente c) =>
        new(c.Id, c.Nome, c.Email, c.Telefone, c.Papel);

    public static QuadraResponseDto ParaDto(this Quadra q) =>
        new(q.Id, q.Nome, q.Descricao, q.ValorHora, q.Status, q.MotivoManutencao);

    public static ReservaResponseDto ParaDto(this Reserva r, string clienteNome, string quadraNome) =>
        new(r.Id, r.ClienteId, clienteNome, r.QuadraId, quadraNome, r.DataHoraInicio, r.DataHoraFim, r.Valor, r.Status);

    /// <summary>Usa as navegações Cliente/Quadra, que precisam estar carregadas.</summary>
    public static ReservaResponseDto ParaDto(this Reserva r) =>
        r.ParaDto(r.Cliente.Nome, r.Quadra.Nome);
}
