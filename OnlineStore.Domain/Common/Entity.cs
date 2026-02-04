using System;
//Am creat o clasă abstractă de bază pentru toate entitățile, care conține identificatorul comun.
//Astfel evit duplicarea codului și respect principiul DRY. Principii: OOP(abstractizare) reutilizare bază pentru LSP”



namespace OnlineStore.Domain.Common
{
    public abstract class Entity
    {
        public Guid Id { get; protected set; }

        protected Entity()
        {
            Id = Guid.NewGuid();
        }
    }
}
