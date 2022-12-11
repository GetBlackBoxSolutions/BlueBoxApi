namespace BlueBoxApi.Models
{
    public class AuthenticatedUserDto
    {
        public UserDto UserProfile { get; set; } = new UserDto();
        public string Token { get; set; } = default!;
    }
}
