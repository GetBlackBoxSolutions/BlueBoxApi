namespace BlueBoxApi.Models
{
    public class AuthenticatedUserDto : UserDto
    {
        public string Token { get; set; }
    }
}
