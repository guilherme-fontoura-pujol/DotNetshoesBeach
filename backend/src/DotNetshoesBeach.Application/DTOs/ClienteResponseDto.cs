namespace DotNetshoesBeach.Application.DTOs;

public record ClienteResponseDto(
    Guid Id,
    string Nome,
    string Email,
    string Telefone
);