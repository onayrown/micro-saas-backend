using MicroSaaS.Shared.Enums;

namespace MicroSaaS.Domain.Entities
{
    public class SocialMediaAccount
    {
        public Guid Id { get; set; }
        public SocialMediaPlatform Platform { get; set; }
        public string AccountUsername { get; set; }
        public string AccessToken { get; set; }
        public DateTime LastSynchronized { get; set; }
    }
}
