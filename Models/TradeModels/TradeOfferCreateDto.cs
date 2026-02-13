namespace DistributedSystemAPI.Models.TradeModels
{
    public class TradeOfferCreateDto
    {
        public int FromUserId { get; set; }
        public int RequestedGameId { get; set; }
        public int OfferedGameId { get; set; }
    }
}
