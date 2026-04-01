using OnlineStore.Domain.DesignPatterns.Behavioral.Memento;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Domain.DesignPatterns.Behavioral.Command
{
    public class CartActionInvoker
    {
        private readonly CartCaretaker _caretaker;

        public CartActionInvoker(CartCaretaker caretaker)
        {
            _caretaker = caretaker;
        }

        public void ExecuteCommand(ICartCommand command, ShoppingCart cart)
        {
            // Păstrăm starea curentă înainte de a executa comanda
            _caretaker.SaveState(cart.SaveState());

            // Executăm comanda care detaliază acțiunea curentă asupra coșului
            command.Execute();
        }

        public void Undo(ShoppingCart cart)
        {
            var previousState = _caretaker.PopUndo();
            if (previousState != null)
            {
                // Salvăm starea actuală în Redo pentru viitor
                _caretaker.PushRedo(cart.SaveState());

                // Restaurăm starea anterioară
                cart.RestoreState(previousState);
            }
        }

        public void Redo(ShoppingCart cart)
        {
            var nextState = _caretaker.PopRedo();
            if (nextState != null)
            {
                // Salvăm starea curentă înapoi în Undo
                _caretaker.PushUndo(cart.SaveState());

                // Restaurăm la starea viitoare (cea refăcută)
                cart.RestoreState(nextState);
            }
        }
    }
}
