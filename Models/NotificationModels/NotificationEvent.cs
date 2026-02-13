namespace DistributedSystemAPI.Models.NotificationModels
{
    public class NotificationEvent
    {
        public string EventType { get; set; } = default!;
        public string RecipientEmail { get; set; } = default!;
        public string RecipientUserId { get; set; } = default!;
        public string Payload { get; set; } = default!;
        public DateTime OccurredAt { get; set; }
    }
}
