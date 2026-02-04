namespace OnlineStore.Domain.Interfaces
{
    public interface IShippingProvider
    {
        void ScheduleShipping(string address);
    }
}
