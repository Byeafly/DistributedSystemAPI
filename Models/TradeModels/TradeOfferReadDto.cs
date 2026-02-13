namespace DistributedSystemAPI.Models.TradeModels
{
    public class TradeOfferReadDto
    {
        public int TradeOfferId { get; set; }
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }
        public int RequestedGameId { get; set; }
        public int OfferedGameId { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public Dictionary<string, string> Links { get; set; }
    }
}
