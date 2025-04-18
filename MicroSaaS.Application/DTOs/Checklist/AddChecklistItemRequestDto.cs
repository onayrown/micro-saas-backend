using System.ComponentModel.DataAnnotations;

namespace MicroSaaS.Application.DTOs.Checklist
{
    public class AddChecklistItemRequestDto
    {
        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Description { get; set; }
        
        public bool IsRequired { get; set; }
    }
}
