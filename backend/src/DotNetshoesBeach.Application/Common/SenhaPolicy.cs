namespace DotNetshoesBeach.Application.Common;

public static class SenhaPolicy
{
    public const int TamanhoMinimo = 8;

    public static void Validar(string? senha)
    {
        if (string.IsNullOrWhiteSpace(senha) || senha.Length < TamanhoMinimo)
            throw new ArgumentException($"A senha deve ter pelo menos {TamanhoMinimo} caracteres.");
    }
}
