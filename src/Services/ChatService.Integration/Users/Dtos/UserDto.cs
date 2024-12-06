namespace ChatService.Integration.Users.Dtos
{

    public record UserDto (string Email)
    {
        public override string ToString() => Email;
    }
}
