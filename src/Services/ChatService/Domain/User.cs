using Microsoft.AspNetCore.Identity;

namespace ChatService.Domain
{
    public class User : IdentityUser
    {
        public List<Conversation> Conversations { get; set; }
        public List<ChatGroup> ChatGroups { get; set; }
    }
}
