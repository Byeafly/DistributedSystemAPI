using Microsoft.AspNetCore.Mvc;
using DistributedSystemAPI.Models.VideoGameModels;
using DistributedSystemAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace DistributedSystemAPI.Controllers
{
    [ApiController]
    [Route("/users/{userId}/games")]
    public class VideoGamesController : ControllerBase
    {
        private readonly ILogger<VideoGamesController> _logger;
        private readonly AppDbContext _context;
        public VideoGamesController(ILogger<VideoGamesController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // POST: /users/{userId}/games
        [HttpPost]
        [ProducesResponseType(typeof(VideoGameReadDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VideoGameReadDto>> CreateGame(int userId, VideoGameCreateDto dto)
        {
            if (!await _context.Users.AnyAsync(u => u.UserId == userId))
                return NotFound("User not found.");

            var validConditions = new[] { "mint", "good", "fair", "poor" };
            if (!validConditions.Contains(dto.Condition))
                return BadRequest("Invalid condition");

            var game = new VideoGame
            {
                UserId = userId,
                Name = dto.Name,
                Publisher = dto.Publisher,
                YearPublished = dto.YearPublished,
                System = dto.System,
                Condition = dto.Condition,
                PreviousOwnersCount = dto.PreviousOwnersCount
            };

            _context.Video_Games.Add(game);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGame),
                new { userId, gameId = game.GameId },
                ToGameDto(game, userId));
        }

        // GET: /users/{userId}/games?name=&system=&publisher=&condition=
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<VideoGameReadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<VideoGameReadDto>>> GetGames(
            int userId,
            string? name,
            string? system,
            string? publisher,
            string? condition)
        {
            if (!await _context.Users.AnyAsync(u => u.UserId == userId))
                return NotFound();

            var query = _context.Video_Games
                .Where(g => g.UserId == userId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(g => g.Name.Contains(name));

            if (!string.IsNullOrEmpty(system))
                query = query.Where(g => g.System.Contains(system));

            if (!string.IsNullOrEmpty(publisher))
                query = query.Where(g => g.Publisher.Contains(publisher));

            if (!string.IsNullOrEmpty(condition))
                query = query.Where(g => g.Condition == condition);

            var games = await query.ToListAsync();

            return Ok(games.Select(g => ToGameDto(g, userId)));
        }


        // GET: /users/{userId}/games/{gameId}
        [HttpGet("{gameId}")]
        [ProducesResponseType(typeof(VideoGameReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VideoGameReadDto>> GetGame(int userId, int gameId)
        {
            var game = await _context.Video_Games
                .FirstOrDefaultAsync(g => g.UserId == userId && g.GameId == gameId);

            if (game == null) return NotFound();

            return Ok(ToGameDto(game, userId));
        }

        // PUT: /users/{userId}/games/{gameId}
        [HttpPut("{gameId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateGame(int userId, int gameId, VideoGameCreateDto dto)
        {
            var game = await _context.Video_Games
                .FirstOrDefaultAsync(g => g.UserId == userId && g.GameId == gameId);

            if (game == null) return NotFound();

            var validConditions = new[] { "mint", "good", "fair", "poor" };
            if (!validConditions.Contains(dto.Condition))
                return BadRequest("Invalid condition");

            game.Name = dto.Name;
            game.Publisher = dto.Publisher;
            game.YearPublished = dto.YearPublished;
            game.System = dto.System;
            game.Condition = dto.Condition;
            game.PreviousOwnersCount = dto.PreviousOwnersCount;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: /users/{userId}/games/{gameId}
        [HttpDelete("{gameId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteGame(int userId, int gameId)
        {
            var game = await _context.Video_Games
                .FirstOrDefaultAsync(g => g.UserId == userId && g.GameId == gameId);

            if (game == null) return NotFound();

            _context.Video_Games.Remove(game);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("/games/search")]
        [ProducesResponseType(typeof(IEnumerable<VideoGameReadDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VideoGameReadDto>>> SearchGames(string? name)
        {
            var query = _context.Video_Games.AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(g => g.Name.Contains(name));

            var games = await query.ToListAsync();

            return Ok(games.Select(g => ToGameDto(g, g.UserId)));
        }

        private VideoGameReadDto ToGameDto(VideoGame game, int userId)
        {
            return new VideoGameReadDto
            {
                GameId = game.GameId,
                Name = game.Name,
                Publisher = game.Publisher,
                YearPublished = game.YearPublished,
                System = game.System,
                Condition = game.Condition,
                PreviousOwnersCount = game.PreviousOwnersCount,
                Links =
                {
                    ["self"] = $"/users/{userId}/games/{game.GameId}",
                    ["update"] = $"/users/{userId}/games/{game.GameId}",
                    ["delete"] = $"/users/{userId}/games/{game.GameId}",
                    ["owner"] = $"/users/{userId}",
                    ["collection"] = $"/users/{userId}/games",
                    ["search"] = $"/games/search?name={game.Name}"
                }
            };
        }
    }
}
