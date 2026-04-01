namespace OnlineStore.Domain.DesignPatterns.Behavioral.Iterator
{
    public interface IProductCollection
    {
        IProductIterator CreateIterator(string? typeFilter);
    }
}
