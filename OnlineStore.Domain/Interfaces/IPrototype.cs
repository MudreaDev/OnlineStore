namespace OnlineStore.Domain.Interfaces
{
    /// <summary>
    /// Interfață pentru Pattern-ul Prototype.
    /// Scop: Permite clonarea unui obiect existent pentru a crea variații fără a reface tot procesul de inițializare.
    /// </summary>
    /// <typeparam name="T">Tipul obiectului generat prin clonare.</typeparam>
    public interface IPrototype<out T>
    {
        T Clone();
    }
}
