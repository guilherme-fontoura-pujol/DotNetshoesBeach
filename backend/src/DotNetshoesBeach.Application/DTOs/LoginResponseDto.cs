namespace DotNetshoesBeach.Application.DTOs;

public record LoginResponseDto(
    string Token,
    DateTime ExpiraEmUtc,
    ClienteResponseDto Usuario
);
