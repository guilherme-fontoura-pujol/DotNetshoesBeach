using DotNetshoesBeach.Domain.Common;
using DotNetshoesBeach.Domain.Enums;

namespace DotNetshoesBeach.Domain.Entities;

public class Reserva : BaseEntity
{
    public Guid ClienteId { get; private set; }
    public Cliente Cliente { get; private set; } = null!;

    public Guid QuadraId { get; private set; }
    public Quadra Quadra { get; private set; } = null!;

    public DateTime DataHoraInicio { get; private set; }
    public DateTime DataHoraFim { get; private set; }
    public StatusReserva Status { get; private set; }

    protected Reserva() { }

    public Reserva(Guid clienteId, Guid quadraId, DateTime dataHoraInicio, DateTime dataHoraFim)
    {
        if (clienteId == Guid.Empty)
            throw new ArgumentException("Cliente inválido.", nameof(clienteId));

        if (quadraId == Guid.Empty)
            throw new ArgumentException("Quadra inválida.", nameof(quadraId));

        if (dataHoraFim <= dataHoraInicio)
            throw new ArgumentException("O horário de término deve ser posterior ao horário de início.");

        ClienteId = clienteId;
        QuadraId = quadraId;
        DataHoraInicio = dataHoraInicio;
        DataHoraFim = dataHoraFim;
        Status = StatusReserva.Confirmada;
    }

    public void Cancelar()
    {
        if (Status == StatusReserva.Cancelada)
            throw new InvalidOperationException("Esta reserva já está cancelada.");

        Status = StatusReserva.Cancelada;
    }
}