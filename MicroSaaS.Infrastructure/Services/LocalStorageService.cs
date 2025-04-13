using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MicroSaaS.Application.Interfaces.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MicroSaaS.Infrastructure.Services
{
    /// <summary>
    /// Implementação do serviço de armazenamento local
    /// </summary>
    public class LocalStorageService : MicroSaaS.Application.Interfaces.Services.IStorageService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl;

        public LocalStorageService(IWebHostEnvironment environment, IConfiguration configuration)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            // Obter a URL base do arquivo de configuração ou usar o padrão
            _baseUrl = _configuration["Storage:BaseUrl"] ?? "https://localhost:7171";
        }

        /// <summary>
        /// Faz upload de um arquivo para o armazenamento local
        /// </summary>
        public async Task<(string Url, string Path)> UploadFileAsync(IFormFile file, string containerName, string fileName = null)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            if (string.IsNullOrWhiteSpace(containerName))
                throw new ArgumentException("O nome do container é obrigatório", nameof(containerName));

            // Gerar um nome de arquivo único se não for fornecido
            fileName = fileName ?? GenerateUniqueFileName(file.FileName);

            // Criar o diretório se não existir
            var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", containerName);
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            // Caminho completo do arquivo
            var filePath = Path.Combine(uploadPath, fileName);

            // Salvar o arquivo
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Caminho relativo para armazenar no banco de dados
            var relativePath = Path.Combine("uploads", containerName, fileName);

            // URL pública do arquivo
            var url = $"{_baseUrl}/uploads/{containerName}/{fileName}";

            return (url, relativePath);
        }

        /// <summary>
        /// Faz upload de um stream para o armazenamento local
        /// </summary>
        public async Task<(string Url, string Path)> UploadStreamAsync(Stream stream, string containerName, string fileName, string contentType)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (string.IsNullOrWhiteSpace(containerName))
                throw new ArgumentException("O nome do container é obrigatório", nameof(containerName));

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("O nome do arquivo é obrigatório", nameof(fileName));

            // Criar o diretório se não existir
            var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", containerName);
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            // Caminho completo do arquivo
            var filePath = Path.Combine(uploadPath, fileName);

            // Salvar o arquivo
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await stream.CopyToAsync(fileStream);
            }

            // Caminho relativo para armazenar no banco de dados
            var relativePath = Path.Combine("uploads", containerName, fileName);

            // URL pública do arquivo
            var url = $"{_baseUrl}/uploads/{containerName}/{fileName}";

            return (url, relativePath);
        }

        /// <summary>
        /// Exclui um arquivo do armazenamento local
        /// </summary>
        public Task<bool> DeleteFileAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("O caminho do arquivo é obrigatório", nameof(filePath));

            // Caminho completo do arquivo
            var fullPath = Path.Combine(_environment.WebRootPath, filePath);

            // Verificar se o arquivo existe
            if (!File.Exists(fullPath))
                return Task.FromResult(false);

            // Excluir o arquivo
            File.Delete(fullPath);

            return Task.FromResult(true);
        }

        /// <summary>
        /// Obtém a URL pública de um arquivo
        /// </summary>
        public Task<string> GetFileUrlAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("O caminho do arquivo é obrigatório", nameof(filePath));

            // Normalizar o caminho
            filePath = filePath.Replace("\\", "/");

            // URL pública do arquivo
            var url = $"{_baseUrl}/{filePath}";

            return Task.FromResult(url);
        }

        /// <summary>
        /// Verifica se um arquivo existe
        /// </summary>
        public Task<bool> FileExistsAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("O caminho do arquivo é obrigatório", nameof(filePath));

            // Caminho completo do arquivo
            var fullPath = Path.Combine(_environment.WebRootPath, filePath);

            // Verificar se o arquivo existe
            var exists = File.Exists(fullPath);

            return Task.FromResult(exists);
        }

        /// <summary>
        /// Gera um nome de arquivo único
        /// </summary>
        public string GenerateUniqueFileName(string originalFileName)
        {
            if (string.IsNullOrWhiteSpace(originalFileName))
                throw new ArgumentException("O nome do arquivo é obrigatório", nameof(originalFileName));

            // Obter a extensão do arquivo
            var extension = Path.GetExtension(originalFileName);

            // Gerar um nome único
            var uniqueName = $"{Guid.NewGuid()}{extension}";

            return uniqueName;
        }
    }
}
