namespace DotNetshoesBeach.Application.Exceptions;

/// <summary>Regra de negócio violada (HTTP 400).</summary>
public class RegraDeNegocioException(string mensagem) : Exception(mensagem);

/// <summary>Recurso não encontrado (HTTP 404).</summary>
public class NaoEncontradoException(string mensagem) : Exception(mensagem);

/// <summary>Conflito com o estado atual, ex.: horário já reservado (HTTP 409).</summary>
public class ConflitoException(string mensagem) : Exception(mensagem);

/// <summary>Credenciais inválidas (HTTP 401).</summary>
public class NaoAutenticadoException(string mensagem) : Exception(mensagem);

/// <summary>Usuário autenticado, mas sem permissão para a operação (HTTP 403).</summary>
public class AcessoNegadoException(string mensagem) : Exception(mensagem);
