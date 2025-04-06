using System;

namespace MicroSaaS.Shared.Results;

/// <summary>
/// Representa o resultado de uma operação, que pode ser bem-sucedida ou falha,
/// com uma mensagem de erro opcional e dados de retorno.
/// </summary>
public class Result<T>
{
    public bool Success { get; private set; }
    public string? ErrorMessage { get; private set; }
    public T? Data { get; private set; }

    private Result(bool success, T? data, string? errorMessage)
    {
        Success = success;
        Data = data;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Cria um resultado de sucesso com os dados fornecidos.
    /// </summary>
    public static Result<T> Ok(T data) => new(true, data, null);

    /// <summary>
    /// Cria um resultado de falha com a mensagem de erro fornecida.
    /// </summary>
    public static Result<T> Fail(string errorMessage) => new(false, default, errorMessage);
    
    /// <summary>
    /// Cria um resultado de falha com a mensagem de erro e dados opcionais.
    /// </summary>
    public static Result<T> Fail(string errorMessage, T? data) => new(false, data, errorMessage);
}

/// <summary>
/// Versão não genérica do Result para operações que não retornam dados.
/// </summary>
public class Result
{
    public bool Success { get; private set; }
    public string? ErrorMessage { get; private set; }

    private Result(bool success, string? errorMessage)
    {
        Success = success;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Cria um resultado de sucesso.
    /// </summary>
    public static Result Ok() => new(true, null);

    /// <summary>
    /// Cria um resultado de falha com a mensagem de erro fornecida.
    /// </summary>
    public static Result Fail(string errorMessage) => new(false, errorMessage);
    
    /// <summary>
    /// Converte este resultado para um Result<T>.
    /// </summary>
    public Result<T> WithData<T>(T data) => Success 
        ? Result<T>.Ok(data) 
        : Result<T>.Fail(ErrorMessage ?? string.Empty);
} 