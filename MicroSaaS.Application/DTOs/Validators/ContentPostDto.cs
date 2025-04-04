﻿using MicroSaaS.Shared.Enums;

namespace MicroSaaS.Application.DTOs.Validators;

public class ContentPostDto
{
    public required string Content { get; set; }
    public SocialMediaPlatform Platform { get; set; }
    public DateTime ScheduledTime { get; set; }
}
