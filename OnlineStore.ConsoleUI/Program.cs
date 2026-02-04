using System;
using System.Collections.Generic;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Interfaces;
using OnlineStore.Domain.Factories;
using OnlineStore.Domain.Strategies;
using OnlineStore.Application.Services;

namespace OnlineStore.ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Online Store - Laborator 2: Creational Patterns ===\n");

            // ---------------------------------------------------------
            // 1. Factory Method Demonstration
            // ---------------------------------------------------------
            Console.WriteLine("--- 1. Factory Method Demo ---");
            
            ProductFactory electronicFactory = new ElectronicProductFactory();
            ProductFactory clothingFactory = new ClothingProductFactory();

            var laptop = electronicFactory.CreateProduct("Gaming Laptop", 1500);
            var tshirt = clothingFactory.CreateProduct("Cotton T-Shirt", 25);

            Console.WriteLine(laptop.GetDescription());
            Console.WriteLine(tshirt.GetDescription());
            Console.WriteLine();

            // ---------------------------------------------------------
            // 2. Abstract Factory Demonstration
            // ---------------------------------------------------------
            Console.WriteLine("--- 2. Abstract Factory Demo ---");

            // Simulation: Choose store type (Local or Global)
            Console.WriteLine("Store Type: Local");
            IStoreServicesFactory localFactory = new LocalStoreServicesFactory();
            DemonstrateStore(localFactory);

            Console.WriteLine("\nStore Type: Global");
            IStoreServicesFactory globalFactory = new GlobalStoreServicesFactory();
            DemonstrateStore(globalFactory);

            // ---------------------------------------------------------
            // 3. Existing SOLID Logic
            // ---------------------------------------------------------
            Console.WriteLine("\n--- 3. Order Service (SOLID) Demo ---");
            var cart = new List<Product> { laptop, tshirt };
            IDiscountStrategy discount = new PercentageDiscountStrategy(10);
            var orderService = new OrderService(discount);

            decimal total = orderService.CalculateTotal(cart);
            Console.WriteLine($"Total Cart Value with 10% Discount: {total:C}");
        }

        static void DemonstrateStore(IStoreServicesFactory factory)
        {
            var payment = factory.CreatePaymentProcessor();
            var shipping = factory.CreateShippingProvider();

            payment.ProcessPayment(100.50m);
            shipping.ScheduleShipping("Str. Libertății nr. 10, București");
        }
    }
}
