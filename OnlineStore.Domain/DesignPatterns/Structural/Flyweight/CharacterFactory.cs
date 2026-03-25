using System.Collections.Generic;

namespace OnlineStore.Domain.DesignPatterns.Structural.Flyweight
{
    // The Flyweight Factory manages the shared objects
    public class CharacterFactory
    {
        private readonly Dictionary<char, Character> _characters = new Dictionary<char, Character>();

        public Character GetCharacter(char key)
        {
            // If the character already exists, return it; otherwise, create a new one
            if (!_characters.ContainsKey(key))
            {
                // In a real scenario, font and size might also be parameters, 
                // but for this example, we use defaults for the shared instance.
                _characters[key] = new Character(key, "Arial", 12);
            }
            return _characters[key];
        }

        public int GetTotalObjectsCreated()
        {
            return _characters.Count;
        }
    }
}
