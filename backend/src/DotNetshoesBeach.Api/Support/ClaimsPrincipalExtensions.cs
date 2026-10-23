using System.Security.Claims;
using DotNetshoesBeach.Application.Common;
using DotNetshoesBeach.Application.Exceptions;

namespace DotNetshoesBeach.Api.Support;

public static class ClaimsPrincipalExtensions
{
    /// <summary>Converte os claims do token JWT na identidade usada pela camada Application.</summary>
    public static UsuarioAtual ObterUsuarioAtual(this ClaimsPrincipal user)
    {
        if (!Guid.TryParse(user.FindFirstValue("sub"), out var id))
            throw new NaoAutenticadoException("Token inválido.");

        return new UsuarioAtual(id, user.IsInRole(Roles.Admin));
    }
}
