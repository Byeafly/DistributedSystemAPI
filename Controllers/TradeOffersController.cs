using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DistributedSystemAPI.Data;
using DistributedSystemAPI.Enums;
using DistributedSystemAPI.Interfaces;
using DistributedSystemAPI.Models.NotificationModels;
using DistributedSystemAPI.Models.TradeModels;
using DistributedSystemAPI.Models.VideoGameModels;

namespace DistributedSystemAPI.Controllers // I used chatgpt to write the logic for this controller
{
    [ApiController]
    [Route("users/{userId}/offers")]
    public class TradeOffersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly INotificationProducer _notificationProducer;

        public TradeOffersController(AppDbContext context, INotificationProducer notificationProducer)
        {
            _context = context;
            _notificationProducer = notificationProducer;
        }

        [HttpPost]
        [ProducesResponseType(typeof(TradeOfferReadDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TradeOfferReadDto>> CreateOffer(int userId, TradeOfferCreateDto dto)
        {
            if (userId != dto.FromUserId)
                return BadRequest("User mismatch.");

            VideoGame? requestedGame = await _context.Video_Games
                .FirstOrDefaultAsync(g => g.GameId == dto.RequestedGameId);

            VideoGame? offeredGame = await _context.Video_Games
                .FirstOrDefaultAsync(g => g.GameId == dto.OfferedGameId && g.UserId == userId);

            if (requestedGame == null || offeredGame == null)
                return NotFound();

            if (requestedGame.UserId == userId)
                return BadRequest("Cannot request your own game.");

            TradeOffer offer = new TradeOffer
            {
                FromUserId = userId,
                ToUserId = requestedGame.UserId,
                RequestedGameId = dto.RequestedGameId,
                OfferedGameId = dto.OfferedGameId
            };

            _context.Trade_Offers.Add(offer);
            await _context.SaveChangesAsync();

            var offeror = await _context.Users.FindAsync(offer.FromUserId);
            var offeree = await _context.Users.FindAsync(offer.ToUserId);

            if (offeror != null && offeree != null)
            {
                // Notify offeror
                await _notificationProducer.ProduceAsync(new NotificationEvent
                {
                    EventType = "OfferCreated",
                    RecipientEmail = offeror.Email,
                    RecipientUserId = offeror.UserId.ToString(),
                    Payload = $"You created a trade offer for {requestedGame.Name}.",
                    OccurredAt = DateTime.UtcNow
                });

                // Notify offeree
                await _notificationProducer.ProduceAsync(new NotificationEvent
                {
                    EventType = "OfferCreated",
                    RecipientEmail = offeree.Email,
                    RecipientUserId = offeree.UserId.ToString(),
                    Payload = $"You received a trade offer for your game {requestedGame.Name}.",
                    OccurredAt = DateTime.UtcNow
                });
            }

            return CreatedAtAction(nameof(GetOffer),
                new { userId = userId, offerId = offer.TradeOfferId },
                MapToReadDto(offer));
        }



        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TradeOfferReadDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TradeOfferReadDto>>> GetOffers(int userId)
        {
            var offers = await _context.Trade_Offers
                .Where(o => o.FromUserId == userId || o.ToUserId == userId)
                .ToListAsync();

            return Ok(offers.Select(MapToReadDto));
        }

        [HttpGet("{offerId}")]
        [ProducesResponseType(typeof(TradeOfferReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<TradeOfferReadDto>> GetOffer(int userId, int offerId)
        {
            var offer = await _context.Trade_Offers.FindAsync(offerId);

            if (offer == null)
                return NotFound();

            if (offer.FromUserId != userId && offer.ToUserId != userId)
                return Forbid();

            return Ok(MapToReadDto(offer));
        }

        [HttpPost("{offerId}/accept")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AcceptOffer(int userId, int offerId)
        {
            var offer = await _context.Trade_Offers
                .Include(o => o.RequestedGame)
                .Include(o => o.OfferedGame)
                .FirstOrDefaultAsync(o => o.TradeOfferId == offerId);

            if (offer == null)
                return NotFound();

            if (offer.ToUserId != userId)
                return Forbid();

            if (offer.Status != TradeOfferStatus.Pending)
                return BadRequest("Offer already processed.");

            var tempOwner = offer.RequestedGame.UserId;

            offer.RequestedGame.UserId = offer.FromUserId;
            offer.OfferedGame.UserId = offer.ToUserId;

            offer.Status = TradeOfferStatus.Accepted;
            offer.RespondedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{offerId}/reject")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> RejectOffer(int userId, int offerId)
        {
            var offer = await _context.Trade_Offers.FindAsync(offerId);

            if (offer == null)
                return NotFound();

            if (offer.ToUserId != userId)
                return Forbid();

            offer.Status = TradeOfferStatus.Rejected;
            offer.RespondedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private TradeOfferReadDto MapToReadDto(TradeOffer offer)
        {
            return new TradeOfferReadDto
            {
                TradeOfferId = offer.TradeOfferId,
                FromUserId = offer.FromUserId,
                ToUserId = offer.ToUserId,
                RequestedGameId = offer.RequestedGameId,
                OfferedGameId = offer.OfferedGameId,
                Status = offer.Status.ToString(),
                CreatedAt = offer.CreatedAt,
                Links = new Dictionary<string, string>
            {
                { "self", $"/users/{offer.FromUserId}/offers/{offer.TradeOfferId}" }
            }
            };
        }
    }

}
