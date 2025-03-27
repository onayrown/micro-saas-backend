using MicroSaaS.Shared.Enums;

namespace MicroSaaS.Domain.Entities
{
    public class ContentPost
    {
        public Guid Id { get; set; }
        public ContentCreator Creator { get; set; }
        public SocialMediaPlatform Platform { get; set; }
        public string Content { get; set; }
        public DateTime ScheduledTime { get; set; }
        public DateTime? PostedTime { get; set; }
        public PostStatus Status { get; set; }
    }
}
