using Microsoft.AspNetCore.Mvc;
using SportZone.DTOs;
using SportZone.Models;
using SportZone.Services;

namespace SportZone.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<UserResponseDto>> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        try
        {
            var user = new User
            {
                Email = createUserDto.Email,
                Password = createUserDto.Password,
                Name = createUserDto.Name,
                PreferredSport = createUserDto.PreferredSport
            };

            var createdUser = await _userService.CreateUserAsync(user);
            
            var response = new UserResponseDto
            {
                Id = createdUser.Id!,
                Email = createdUser.Email,
                Name = createdUser.Name,
                PreferredSport = createdUser.PreferredSport,
                CreatedAt = createdUser.CreatedAt
            };

            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return StatusCode(500, "An error occurred while creating the user");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponseDto>> GetUserById(string id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        
        if (user == null)
        {
            return NotFound($"User with ID {id} not found");
        }

        var response = new UserResponseDto
        {
            Id = user.Id!,
            Email = user.Email,
            Name = user.Name,
            PreferredSport = user.PreferredSport,
            CreatedAt = user.CreatedAt
        };

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        
        var response = users.Select(u => new UserResponseDto
        {
            Id = u.Id!,
            Email = u.Email,
            Name = u.Name,
            PreferredSport = u.PreferredSport,
            CreatedAt = u.CreatedAt
        });

        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateUser(string id, [FromBody] UpdateUserDto updateUserDto)
    {
        var existingUser = await _userService.GetUserByIdAsync(id);
        
        if (existingUser == null)
        {
            return NotFound($"User with ID {id} not found");
        }

        existingUser.Email = updateUserDto.Email ?? existingUser.Email;
        existingUser.Name = updateUserDto.Name ?? existingUser.Name;
        existingUser.PreferredSport = updateUserDto.PreferredSport ?? existingUser.PreferredSport;
        
        if (!string.IsNullOrEmpty(updateUserDto.Password))
        {
            existingUser.Password = updateUserDto.Password;
        }

        var result = await _userService.UpdateUserAsync(id, existingUser);

        if (!result)
        {
            return StatusCode(500, "Failed to update user");
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(string id)
    {
        var result = await _userService.DeleteUserAsync(id);

        if (!result)
        {
            return NotFound($"User with ID {id} not found");
        }

        return NoContent();
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        var isValid = await _userService.ValidateUserCredentialsAsync(loginDto.Email, loginDto.Password);

        if (!isValid)
        {
            return Unauthorized("Invalid email or password");
        }

        var user = await _userService.GetUserByEmailAsync(loginDto.Email);
        
        var response = new UserResponseDto
        {
            Id = user!.Id!,
            Email = user.Email,
            Name = user.Name,
            PreferredSport = user.PreferredSport,
            CreatedAt = user.CreatedAt
        };

        return Ok(response);
    }
}
