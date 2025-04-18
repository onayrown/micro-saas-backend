using System.ComponentModel.DataAnnotations;

namespace MicroSaaS.Application.DTOs.Checklist
{
    public class UpdateItemRequestDto
    {
        [Required]
        public bool IsCompleted { get; set; }
    }
}
