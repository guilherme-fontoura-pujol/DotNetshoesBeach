using DotNetshoesBeach.Domain.Common;
using DotNetshoesBeach.Domain.Enums;

namespace DotNetshoesBeach.Domain.Entities;

public class Reserva : BaseEntity
{
    public static readonly TimeSpan DuracaoMinima = TimeSpan.FromHours(1);
    public static readonly TimeSpan DuracaoMaxima = TimeSpan.FromHours(4);

    public Guid ClienteId { get; private set; }
    public Cliente Cliente { get; private set; } = null!;

    public Guid QuadraId { get; private set; }
    public Quadra Quadra { get; private set; } = null!;

    /// <summary>Sempre em UTC.</summary>
    public DateTime DataHoraInicio { get; private set; }

    /// <summary>Sempre em UTC.</summary>
    public DateTime DataHoraFim { get; private set; }

    /// <summary>
    /// Valor cobrado, calculado na criação (duração × valor/hora da quadra naquele momento).
    /// Fica gravado na reserva: se o preço da quadra mudar depois, o histórico não se altera.
    /// </summary>
    public decimal Valor { get; private set; }

    public StatusReserva Status { get; private set; }

    protected Reserva() { }

    public Reserva(Guid clienteId, Guid quadraId, DateTime dataHoraInicioUtc, DateTime dataHoraFimUtc,
        decimal valorHora, DateTime agoraUtc)
    {
        if (clienteId == Guid.Empty)
            throw new ArgumentException("Cliente inválido.", nameof(clienteId));

        if (quadraId == Guid.Empty)
            throw new ArgumentException("Quadra inválida.", nameof(quadraId));

        if (dataHoraFimUtc <= dataHoraInicioUtc)
            throw new ArgumentException("O horário de término deve ser posterior ao horário de início.");

        if (dataHoraInicioUtc <= agoraUtc)
            throw new ArgumentException("A reserva deve começar em um horário futuro.");

        var duracao = dataHoraFimUtc - dataHoraInicioUtc;
        if (duracao < DuracaoMinima || duracao > DuracaoMaxima)
            throw new ArgumentException(
                $"A duração da reserva deve ser de {DuracaoMinima.TotalHours:0} a {DuracaoMaxima.TotalHours:0} horas.");

        if (valorHora <= 0)
            throw new ArgumentException("O valor por hora deve ser maior que zero.", nameof(valorHora));

        ClienteId = clienteId;
        QuadraId = quadraId;
        DataHoraInicio = DateTime.SpecifyKind(dataHoraInicioUtc, DateTimeKind.Utc);
        DataHoraFim = DateTime.SpecifyKind(dataHoraFimUtc, DateTimeKind.Utc);
        Valor = Math.Round(valorHora * (decimal)duracao.TotalHours, 2, MidpointRounding.AwayFromZero);
        Status = StatusReserva.Confirmada;
    }

    /// <param name="permitirJaIniciada">Admin pode cancelar/limpar reservas que já começaram.</param>
    public void Cancelar(DateTime agoraUtc, bool permitirJaIniciada = false)
    {
        if (Status == StatusReserva.Cancelada)
            throw new InvalidOperationException("Esta reserva já está cancelada.");

        if (!permitirJaIniciada && DataHoraInicio <= agoraUtc)
            throw new InvalidOperationException("Não é possível cancelar uma reserva que já começou.");

        Status = StatusReserva.Cancelada;
    }
}
