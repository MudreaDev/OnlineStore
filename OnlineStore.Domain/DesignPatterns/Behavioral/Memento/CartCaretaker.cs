using System.Collections.Generic;

namespace OnlineStore.Domain.DesignPatterns.Behavioral.Memento
{
    public class CartCaretaker
    {
        // Stack for undo actions
        public Stack<CartMemento> UndoStack { get; set; } = new Stack<CartMemento>();

        // Stack for redo actions
        public Stack<CartMemento> RedoStack { get; set; } = new Stack<CartMemento>();

        public void SaveState(CartMemento memento)
        {
            UndoStack.Push(memento);
            // Whenever a new state is saved, the redo history is cleared
            RedoStack.Clear();
        }

        public CartMemento? PopUndo()
        {
            if (UndoStack.Count > 0)
            {
                return UndoStack.Pop();
            }
            return null;
        }

        public CartMemento? PopRedo()
        {
            if (RedoStack.Count > 0)
            {
                return RedoStack.Pop();
            }
            return null;
        }

        public void PushRedo(CartMemento memento)
        {
            RedoStack.Push(memento);
        }

        // Needed when undoing: we save the current state to the redo stack, then restore the popped one
        public void PushUndo(CartMemento memento)
        {
            UndoStack.Push(memento);
        }
    }
}
