using System;
using System.Text.Json.Serialization;

namespace MicroSaaS.Shared.DTOs
{
    /// <summary>
    /// DTO para representar uma mídia
    /// </summary>
    public class MediaDto
    {
        /// <summary>
        /// ID único da mídia
        /// </summary>
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// ID do criador de conteúdo que fez o upload
        /// </summary>
        [JsonPropertyName("creatorId")]
        public Guid CreatorId { get; set; }

        /// <summary>
        /// URL da mídia
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Nome do arquivo original
        /// </summary>
        [JsonPropertyName("fileName")]
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Tipo do arquivo (MIME type)
        /// </summary>
        [JsonPropertyName("fileType")]
        public string FileType { get; set; } = string.Empty;

        /// <summary>
        /// Tamanho do arquivo em bytes
        /// </summary>
        [JsonPropertyName("fileSize")]
        public long FileSize { get; set; }

        /// <summary>
        /// Largura da imagem ou vídeo (se aplicável)
        /// </summary>
        [JsonPropertyName("width")]
        public int? Width { get; set; }

        /// <summary>
        /// Altura da imagem ou vídeo (se aplicável)
        /// </summary>
        [JsonPropertyName("height")]
        public int? Height { get; set; }

        /// <summary>
        /// Duração do vídeo ou áudio em segundos (se aplicável)
        /// </summary>
        [JsonPropertyName("duration")]
        public double? Duration { get; set; }

        /// <summary>
        /// Data de criação da mídia
        /// </summary>
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Data da última atualização da mídia
        /// </summary>
        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Indica se a mídia está ativa
        /// </summary>
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; } = true;
    }
}
