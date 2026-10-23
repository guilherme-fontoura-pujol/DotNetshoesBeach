using DotNetshoesBeach.Domain.Enums;

namespace DotNetshoesBeach.Application.DTOs;

/// <param name="ClienteId">Só é considerado quando quem chama é admin (reservar em nome de um cliente).</param>
public record CriarReservaDto(
    Guid QuadraId,
    DateTimeOffset DataHoraInicio,
    DateTimeOffset DataHoraFim,
    Guid? ClienteId = null
);

public record ReservaResponseDto(
    Guid Id,
    Guid ClienteId,
    string ClienteNome,
    Guid QuadraId,
    string QuadraNome,
    DateTime DataHoraInicio,
    DateTime DataHoraFim,
    decimal Valor,
    StatusReserva Status
);

public record FiltroReservaDto(
    Guid? QuadraId = null,
    Guid? ClienteId = null,
    DateTimeOffset? De = null,
    DateTimeOffset? Ate = null,
    StatusReserva? Status = null,
    int Pagina = 1,
    int TamanhoPagina = 20
);
