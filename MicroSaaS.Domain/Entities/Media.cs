using System;

namespace MicroSaaS.Domain.Entities
{
    /// <summary>
    /// Entidade que representa uma mídia (imagem, vídeo, etc.)
    /// </summary>
    public class Media
    {
        /// <summary>
        /// ID único da mídia
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID do criador de conteúdo que fez o upload
        /// </summary>
        public Guid CreatorId { get; set; }

        /// <summary>
        /// URL da mídia
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Nome do arquivo original
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Tipo do arquivo (MIME type)
        /// </summary>
        public string FileType { get; set; } = string.Empty;

        /// <summary>
        /// Tamanho do arquivo em bytes
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// Caminho do arquivo no armazenamento
        /// </summary>
        public string StoragePath { get; set; } = string.Empty;

        /// <summary>
        /// Largura da imagem ou vídeo (se aplicável)
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// Altura da imagem ou vídeo (se aplicável)
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// Duração do vídeo ou áudio em segundos (se aplicável)
        /// </summary>
        public double? Duration { get; set; }

        /// <summary>
        /// Data de criação da mídia
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Data da última atualização da mídia
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Indica se a mídia está ativa
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public Media()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Construtor com parâmetros
        /// </summary>
        public Media(Guid creatorId, string url, string fileName, string fileType, long fileSize, string storagePath)
        {
            Id = Guid.NewGuid();
            CreatorId = creatorId;
            Url = url;
            FileName = fileName;
            FileType = fileType;
            FileSize = fileSize;
            StoragePath = storagePath;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Atualiza os metadados da mídia
        /// </summary>
        public void UpdateMetadata(int? width, int? height, double? duration)
        {
            Width = width;
            Height = height;
            Duration = duration;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Marca a mídia como inativa (excluída logicamente)
        /// </summary>
        public void Delete()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
