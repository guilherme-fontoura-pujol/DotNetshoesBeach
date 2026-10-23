namespace DotNetshoesBeach.Application.Common;

/// <summary>Identidade de quem está chamando a API (extraída do token JWT).</summary>
public record UsuarioAtual(Guid Id, bool EhAdmin);
