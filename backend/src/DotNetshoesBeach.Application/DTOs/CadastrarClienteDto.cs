namespace DotNetshoesBeach.Application.DTOs;

public record CadastrarClienteDto(
    string Nome,
    string Email,
    string Telefone,
    string Senha
);