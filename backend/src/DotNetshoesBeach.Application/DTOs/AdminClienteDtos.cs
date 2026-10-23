using DotNetshoesBeach.Domain.Enums;

namespace DotNetshoesBeach.Application.DTOs;

/// <summary>Criação de conta feita por um admin (pode definir o papel).</summary>
public record AdminCriarClienteDto(
    string Nome,
    string Email,
    string Telefone,
    string Senha,
    Papel Papel = Papel.Cliente
);

/// <summary>Atualização de conta. Papel e NovaSenha são opcionais.</summary>
public record AtualizarClienteDto(
    string Nome,
    string Email,
    string Telefone,
    Papel? Papel = null,
    string? NovaSenha = null
);
