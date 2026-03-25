using System;

namespace OnlineStore.Domain.DesignPatterns.Structural.Flyweight
{
    // The Flyweight class contains the intrinsic state (shared data)
    public class Character
    {
        public char Symbol { get; }
        public string Font { get; }
        public int Size { get; }

        public Character(char symbol, string font, int size)
        {
            Symbol = symbol;
            Font = font;
            Size = size;
        }

        public void Display(int pointSize)
        {
            // pointSize is extrinsic state (passed by the client)
            Console.WriteLine($"{Symbol} (font: {Font}, base size: {Size}, display size: {pointSize})");
        }
    }
}
