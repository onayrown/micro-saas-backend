using System;
using System.ComponentModel.DataAnnotations;

namespace MicroSaaS.Application.DTOs.Checklist
{
    public class SetReminderRequestDto
    {
        [Required]
        public DateTime ReminderDate { get; set; }
    }
}
