using DotNetshoesBeach.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace DotNetshoesBeach.Api.Support;

/// <summary>Converte exceções de negócio em respostas HTTP { "mensagem": "..." }.</summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, mensagem) = exception switch
        {
            NaoAutenticadoException e => (StatusCodes.Status401Unauthorized, e.Message),
            AcessoNegadoException e => (StatusCodes.Status403Forbidden, e.Message),
            NaoEncontradoException e => (StatusCodes.Status404NotFound, e.Message),
            ConflitoException e => (StatusCodes.Status409Conflict, e.Message),
            RegraDeNegocioException e => (StatusCodes.Status400BadRequest, e.Message),
            // Validações do domínio (construtores / métodos das entidades).
            ArgumentException e => (StatusCodes.Status400BadRequest, e.Message),
            InvalidOperationException e => (StatusCodes.Status400BadRequest, e.Message),
            _ => (StatusCodes.Status500InternalServerError, "Ocorreu um erro interno. Tente novamente mais tarde.")
        };

        if (status == StatusCodes.Status500InternalServerError)
            _logger.LogError(exception, "Erro não tratado em {Metodo} {Caminho}", httpContext.Request.Method, httpContext.Request.Path);

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(new { mensagem }, cancellationToken);
        return true;
    }
}
