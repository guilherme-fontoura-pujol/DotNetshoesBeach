using DotNetshoesBeach.Domain.Enums;

namespace DotNetshoesBeach.Application.DTOs;

public record SalvarQuadraDto(
    string Nome,
    string? Descricao,
    decimal ValorHora
);

public record ColocarEmManutencaoDto(
    string? Motivo,
    bool CancelarReservas = false
);

public record QuadraResponseDto(
    Guid Id,
    string Nome,
    string Descricao,
    decimal ValorHora,
    StatusQuadra Status,
    string? MotivoManutencao
);

/// <summary>Intervalo já reservado (sem dados do cliente), usado pelo painel para mostrar horários livres.</summary>
public record PeriodoOcupadoDto(
    DateTime DataHoraInicio,
    DateTime DataHoraFim
);
