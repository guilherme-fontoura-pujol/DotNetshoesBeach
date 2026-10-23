using DotNetshoesBeach.Domain.Entities;

namespace DotNetshoesBeach.Application.Interfaces;

public record TokenGerado(string Token, DateTime ExpiraEmUtc);

public interface ITokenService
{
    TokenGerado GerarToken(Cliente usuario);
}
