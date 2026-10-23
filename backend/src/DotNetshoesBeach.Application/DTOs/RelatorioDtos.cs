namespace DotNetshoesBeach.Application.DTOs;

/// <summary>Resumo de uma quadra em um período (somente reservas confirmadas).</summary>
public record ResumoQuadraDto(
    Guid QuadraId,
    string QuadraNome,
    int QuantidadeReservas,
    decimal ValorTotal,
    decimal ValorMedio
);
