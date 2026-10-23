namespace DotNetshoesBeach.Application.Interfaces;

public interface IUnitOfWork
{
    /// <summary>
    /// Persiste as alterações. Violações de unicidade / sobreposição de horário no banco
    /// são traduzidas para <see cref="Exceptions.ConflitoException"/>.
    /// </summary>
    Task SalvarAsync(CancellationToken cancellationToken = default);
}
