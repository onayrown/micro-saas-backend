using System;
using System.IO;
using System.Threading.Tasks;

namespace MicroSaaS.Domain.Interfaces.Services
{
    /// <summary>
    /// Interface para o serviço de armazenamento de arquivos (versão Domain)
    /// </summary>
    public interface IStorageService
    {
        /// <summary>
        /// Faz upload de um stream para o armazenamento
        /// </summary>
        /// <param name="stream">Stream do arquivo</param>
        /// <param name="containerName">Nome do container/pasta</param>
        /// <param name="fileName">Nome do arquivo</param>
        /// <param name="contentType">Tipo de conteúdo (MIME type)</param>
        /// <returns>URL pública do arquivo e caminho de armazenamento</returns>
        Task<(string Url, string Path)> UploadStreamAsync(Stream stream, string containerName, string fileName, string contentType);

        /// <summary>
        /// Exclui um arquivo do armazenamento
        /// </summary>
        /// <param name="filePath">Caminho do arquivo</param>
        /// <returns>True se excluído com sucesso, False caso contrário</returns>
        Task<bool> DeleteFileAsync(string filePath);

        /// <summary>
        /// Obtém a URL pública de um arquivo
        /// </summary>
        /// <param name="filePath">Caminho do arquivo</param>
        /// <returns>URL pública do arquivo</returns>
        Task<string> GetFileUrlAsync(string filePath);

        /// <summary>
        /// Verifica se um arquivo existe
        /// </summary>
        /// <param name="filePath">Caminho do arquivo</param>
        /// <returns>True se o arquivo existe, False caso contrário</returns>
        Task<bool> FileExistsAsync(string filePath);

        /// <summary>
        /// Gera um nome de arquivo único
        /// </summary>
        /// <param name="originalFileName">Nome original do arquivo</param>
        /// <returns>Nome de arquivo único</returns>
        string GenerateUniqueFileName(string originalFileName);
    }
}
