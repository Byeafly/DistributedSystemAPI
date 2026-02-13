using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DistributedSystemAPI.Data;
using DistributedSystemAPI.Interfaces;
using DistributedSystemAPI.Models.NotificationModels;
using DistributedSystemAPI.Models.UserModels;

namespace DistributedSystemAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly AppDbContext _context;
        private readonly INotificationProducer _notificationProducer;

        public UsersController(ILogger<UsersController> logger, AppDbContext context, INotificationProducer notificationProducer)
        {
            _logger = logger;
            _context = context;
            _notificationProducer = notificationProducer;
        }

        // POST: /users (Self-register)
        [HttpPost]
        [ProducesResponseType(typeof(UserReadDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserReadDto>> Register(UserCreateDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Email already exists");

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                StreetAddress = dto.StreetAddress
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, ToUserDto(user));
        }

        // GET: /users/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserReadDto>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            return Ok(ToUserDto(user));
        }

        // PUT: /users/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateUser(int id, UserUpdateDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            if (string.IsNullOrEmpty(dto.Name) || string.IsNullOrEmpty(dto.StreetAddress))
                return BadRequest("Name and StreetAddress are required");

            user.Name = dto.Name;
            user.StreetAddress = dto.StreetAddress;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PUT: /users/{id}/password
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{id}/password")]
        public async Task<IActionResult> UpdatePassword(int id, UserUpdatePasswordDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                return BadRequest("Current password incorrect");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            await _context.SaveChangesAsync();

            await _notificationProducer.ProduceAsync(new NotificationEvent
            {
                EventType = "PasswordChanged",
                RecipientEmail = user.Email,
                RecipientUserId = user.UserId.ToString(),
                Payload = "Your password was successfully changed.",
                OccurredAt = DateTime.UtcNow
            });

            return NoContent();
        }



        private UserReadDto ToUserDto(User user)
        {
            return new UserReadDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                StreetAddress = user.StreetAddress,
                Links =
                {
                    ["self"] = $"/users/{user.UserId}",
                    ["update"] = $"/users/{user.UserId}",
                    ["games"] = $"/users/{user.UserId}/games"
                }
            };
        }
    }
}
