using System;
using System.ComponentModel.DataAnnotations;

namespace MicroSaaS.Application.DTOs.Checklist
{
    public class CreateChecklistRequestDto
    {
        [Required]
        public Guid CreatorId { get; set; }
        
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Title { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; }
    }
}
