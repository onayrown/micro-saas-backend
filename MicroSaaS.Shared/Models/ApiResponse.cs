using System.Text.Json.Serialization;

namespace MicroSaaS.Shared.Models;

/// <summary>
/// Modelo padrão para respostas de API
/// </summary>
/// <typeparam name="T">O tipo de dados retornado na resposta</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Indica se a operação foi bem-sucedida
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// Mensagem associada à resposta (geralmente em caso de erro)
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    /// <summary>
    /// Os dados retornados pela operação
    /// </summary>
    [JsonPropertyName("data")]
    public T? Data { get; set; }

    /// <summary>
    /// Cria uma resposta de sucesso com os dados fornecidos
    /// </summary>
    public static ApiResponse<T> SuccessResponse(T data)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data
        };
    }

    /// <summary>
    /// Cria uma resposta de falha com a mensagem de erro fornecida
    /// </summary>
    public static ApiResponse<T> FailureResponse(string message)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message
        };
    }
} 