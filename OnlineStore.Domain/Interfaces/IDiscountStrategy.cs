namespace OnlineStore.Domain.Interfaces
{
    // Strategy Pattern:
    // Definește contractul pentru algoritmii de discount
    // OCP: putem adăuga strategii noi fără a modifica codul existent
    public interface IDiscountStrategy
    {
        decimal ApplyDiscount(decimal price);
    }
}
