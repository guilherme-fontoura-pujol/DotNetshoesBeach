namespace DotNetshoesBeach.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CriadoEm { get; protected set; } = DateTime.UtcNow;
}