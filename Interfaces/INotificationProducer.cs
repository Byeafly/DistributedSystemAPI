using DistributedSystemAPI.Models.NotificationModels;

namespace DistributedSystemAPI.Interfaces
{
    public interface INotificationProducer
    {
        Task ProduceAsync(NotificationEvent notification);
    }
}
