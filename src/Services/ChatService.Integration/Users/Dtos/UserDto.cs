namespace ChatService.Integration.Users.Dtos
{

    public record UserDto (string Email, bool IsOnline)
    {
        public override string ToString() => Email + (IsOnline ? " - Online": " - Offline");
    }
}
