namespace DotNetshoesBeach.Infrastructure.Options;

public class JwtOptions
{
    public const string Secao = "Jwt";

    /// <summary>Chave de assinatura (HMAC-SHA256). Mínimo de 32 caracteres. Nunca versionar a chave real.</summary>
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = "DotNetshoesBeach";
    public string Audience { get; set; } = "DotNetshoesBeach.Frontend";
    public int ExpiracaoMinutos { get; set; } = 120;
}

public class AdminInicialOptions
{
    public const string Secao = "AdminInicial";

    public string Nome { get; set; } = "Administrador";
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}
