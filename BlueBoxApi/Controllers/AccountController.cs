using BlueBoxApi.Identity;
using BlueBoxApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlueBoxApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IdentityTokenClaimService _tokenService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IdentityTokenClaimService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Get the current users information
        /// </summary>
        /// <returns>Returns a UserDto</returns>
        /// <response code="200">Returns a Current User Object</response>
        /// <response code="401">Returns if the user is not athorized</response>
        [Authorize]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("CurrentUser")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));
            if (user == null) return NotFound();
            return new UserDto()
            {
                DisplayName = user.DisplayName,
                UserName = user.UserName
            };
        }

        /// <summary>
        /// Get the current users access token
        /// </summary>
        /// <returns>Returns a UserDto</returns>
        /// <response code="200">Returns the current users access token</response>
        /// <response code="404">Returns if the current user is not found</response>
        /// <response code="401">Returns if the user is not athorized</response>
        [Authorize]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("AccessToken")]
        public async Task<ActionResult<string>> GetAccessToken()
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));
            if (user == null) return NotFound();
            return await _tokenService.CreateTokenAsync(user.UserName);
        }

        /// <summary>
        /// Post a login request
        /// </summary>
        /// <returns>Returns a AuthenticatedUserDto</returns>
        /// <response code="200">Returns the current users profile and token</response>        
        /// <response code="401">Returns if the user is not athorized</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]     
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult<AuthenticatedUserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null) return Unauthorized();

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            
            if (result.Succeeded)
            {
                return new AuthenticatedUserDto()
                {
                    UserProfile = new UserDto()
                    {
                        DisplayName = user.DisplayName,
                        UserName = user.UserName,
                    },
                    Token = await _tokenService.CreateTokenAsync(user.UserName)
                };
            }

            return Unauthorized();
        }

        /// <summary>
        /// Post a new user request
        /// </summary>
        /// <returns>Returns a AuthenticatedUserDto</returns>
        /// <response code="200">Returns the current users profile and token</response>        
        /// <response code="400">Returns if the user failed posting the correct information</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<ActionResult<AuthenticatedUserDto>> Register(RegisterDto registerDto)
        {
            if (await _userManager.Users.AnyAsync(x => x.Email == registerDto.Email))
            {
                return BadRequest("Email Taken");
            }

            if (await _userManager.Users.AnyAsync(x => x.UserName == registerDto.UserName))
            {
                return BadRequest("Username Taken");
            }

            var user = new ApplicationUser()
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                UserName = registerDto.UserName,
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded)
            {
                return new AuthenticatedUserDto()
                {
                    UserProfile = new UserDto()
                    {
                        DisplayName = user.DisplayName,
                        UserName = user.UserName,
                    },                    
                    Token = await _tokenService.CreateTokenAsync(user.UserName)
                };
            }

            return BadRequest("Problem registering user");
        }
        
    }
}
