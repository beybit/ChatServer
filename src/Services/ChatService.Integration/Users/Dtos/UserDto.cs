namespace ChatService.Integration.Users.Dtos
{

    public class UserDto
    {
        public string Email { get; set; }

        public override string ToString() => Email;
    }
}
