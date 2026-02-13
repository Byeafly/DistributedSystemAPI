using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DistributedSystemAPI.Models.TradeModels;
using DistributedSystemAPI.Models.UserModels;

namespace DistributedSystemAPI.Models.VideoGameModels
{
    public class VideoGame
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("game_id")]
        public int GameId { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("name")]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        [Column("publisher")]
        public string Publisher { get; set; } = null!;

        [Required]
        [Column("year_published")]
        public int YearPublished { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("system")]
        public string System { get; set; } = null!;

        [Column("previous_owners_count")]
        public int? PreviousOwnersCount { get; set; }

        [Required]
        [MaxLength(10)]
        [RegularExpression("mint|good|fair|poor")]
        [Column("condition")]
        public string Condition { get; set; } = null!;

        // Navigation property
        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        // Trade navigation
        public ICollection<TradeOffer> RequestedInOffers { get; set; } = new List<TradeOffer>();
        public ICollection<TradeOffer> OfferedInOffers { get; set; } = new List<TradeOffer>();
    }
}
