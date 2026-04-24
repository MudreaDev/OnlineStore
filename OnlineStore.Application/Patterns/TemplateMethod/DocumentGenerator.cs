using OnlineStore.Domain.Entities;
using System;
using System.Text;

namespace OnlineStore.Application.Patterns.TemplateMethod
{
    public abstract class DocumentGenerator
    {
        protected StringBuilder _content = new StringBuilder();

        // Template Method
        public string Generate(Order order)
        {
            _content.Clear();
            _content.AppendLine($"=== GENERATING {GetType().Name.ToUpper()} ===");
            
            AddHeader(order);
            AddContent(order);
            AddFooter(order);
            
            Save();
            
            return _content.ToString();
        }

        protected abstract void AddHeader(Order order);
        protected abstract void AddContent(Order order);
        protected abstract void AddFooter(Order order);

        protected virtual void Save()
        {
            // Logic for saving to a database or file system would go here
            Console.WriteLine($"Saving {GetType().Name} to storage...");
        }
    }
}
