using System;
using System.ComponentModel.DataAnnotations;

namespace MicroSaaS.Application.DTOs.Checklist
{
    public class SetDueDateRequestDto
    {
        [Required]
        public DateTime DueDate { get; set; }
    }
}
