namespace BlueBoxApi.Models
{
    public class AuthenticatedUserDto
    {
        public UserDto UserProfile { get; set; }
        public string Token { get; set; }
    }
}
