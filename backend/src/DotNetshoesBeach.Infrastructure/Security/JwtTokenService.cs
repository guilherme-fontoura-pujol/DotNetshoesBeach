using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DotNetshoesBeach.Application.Interfaces;
using DotNetshoesBeach.Domain.Entities;
using DotNetshoesBeach.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DotNetshoesBeach.Infrastructure.Security;

public class JwtTokenService : ITokenService
{
    private readonly JwtOptions _options;
    private readonly TimeProvider _clock;

    public JwtTokenService(IOptions<JwtOptions> options, TimeProvider clock)
    {
        _options = options.Value;
        _clock = clock;
    }

    public TokenGerado GerarToken(Cliente usuario)
    {
        var agora = _clock.GetUtcNow().UtcDateTime;
        var expira = agora.AddMinutes(_options.ExpiracaoMinutos);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, usuario.Email),
            new(JwtRegisteredClaimNames.Name, usuario.Nome),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            // O nome do claim "role" é o mesmo configurado em RoleClaimType no Program.cs da API.
            new("role", usuario.Papel.ToString())
        };

        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: agora,
            expires: expira,
            signingCredentials: credenciais);

        return new TokenGerado(new JwtSecurityTokenHandler().WriteToken(jwt), expira);
    }
}
