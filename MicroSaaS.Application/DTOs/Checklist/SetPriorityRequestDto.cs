using System.ComponentModel.DataAnnotations;
using MicroSaaS.Shared.Enums;

namespace MicroSaaS.Application.DTOs.Checklist
{
    public class SetPriorityRequestDto
    {
        [Required]
        public TaskPriority Priority { get; set; }
    }
}
