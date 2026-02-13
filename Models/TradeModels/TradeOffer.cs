using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DistributedSystemAPI.Enums;
using DistributedSystemAPI.Models.UserModels;
using DistributedSystemAPI.Models.VideoGameModels;

namespace DistributedSystemAPI.Models.TradeModels
{
    public class TradeOffer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("trade_offer_id")]
        public int TradeOfferId { get; set; }

        [Required]
        [Column("from_user_id")]
        public int FromUserId { get; set; }

        [Required]
        [Column("to_user_id")]
        public int ToUserId { get; set; }

        [Required]
        [Column("requested_game_id")]
        public int RequestedGameId { get; set; }

        [Required]
        [Column("offered_game_id")]
        public int OfferedGameId { get; set; }

        [Required]
        [Column("status")]
        public TradeOfferStatus Status { get; set; } = TradeOfferStatus.Pending;

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("responded_at")]
        public DateTime? RespondedAt { get; set; }

        // Navigation Properties

        [ForeignKey(nameof(FromUserId))]
        public User FromUser { get; set; } = null!;

        [ForeignKey(nameof(ToUserId))]
        public User ToUser { get; set; } = null!;

        [ForeignKey(nameof(RequestedGameId))]
        public VideoGame RequestedGame { get; set; } = null!;

        [ForeignKey(nameof(OfferedGameId))]
        public VideoGame OfferedGame { get; set; } = null!;
    }
}
