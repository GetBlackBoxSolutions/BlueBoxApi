using Microsoft.AspNetCore.Identity;

namespace BlueBoxApi.Identity
{
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Display Name
        /// </summary>
        public string DisplayName { get; set; } = default!;
    }
}
