using System;
using System.Collections.Generic;
using System.Linq;
using OnlineStore.Application.Repositories;
using OnlineStore.Application.Services;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Enums;
using OnlineStore.Domain.Factories;
using OnlineStore.Domain.Interfaces;
using OnlineStore.Domain.Strategies;

namespace OnlineStore.ConsoleUI
{
    class Program
    {
        static InMemoryProductRepository productRepo = new InMemoryProductRepository();
        static InMemoryUserRepository userRepo = new InMemoryUserRepository();
        static InMemoryOrderRepository orderRepo = new InMemoryOrderRepository();

        static User currentUser = null;
        static ShoppingCart currentCart = null;

        static void Main(string[] args)
        {
            // Seed data
            SeedData();

            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("Online Store Demo");
                Console.WriteLine("-----------------");
                Console.WriteLine("1. Guest (Browse Products)");
                Console.WriteLine("2. Customer (Place Orders)");
                Console.WriteLine("3. Admin (Manage Products)");
                Console.WriteLine("0. Exit");
                Console.Write("Select option: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        GuestMenu();
                        break;
                    case "2":
                        CustomerMenu();
                        break;
                    case "3":
                        AdminMenu();
                        break;
                    case "0":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Press any key...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void SeedData()
        {
            // Add some products using factories
            var electronicsFactory = new ElectronicProductFactory();
            productRepo.Add(electronicsFactory.CreateProduct("Laptop", 2500m));
            productRepo.Add(electronicsFactory.CreateProduct("Smartphone", 1200m));

            // Add users
            userRepo.Add(new Customer("johndoe", "john@example.com", "123 Main St"));
            userRepo.Add(new Admin("admin", "admin@store.com", "SuperAdmin"));
        }

        static void GuestMenu()
        {
            Console.Clear();
            Console.WriteLine("Guest Menu - Browsing Products");
            var products = productRepo.GetAll();
            foreach (var p in products)
            {
                Console.WriteLine($"{p.Name} - E{p.Price}");
            }
            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        static void CustomerMenu()
        {
            Console.Clear();
            Console.WriteLine("Customer Login (Select User):");
            var customers = userRepo.GetAll().OfType<Customer>().ToList();
            for (int i = 0; i < customers.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {customers[i].Username}");
            }

            Console.Write("Select customer: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= customers.Count)
            {
                currentUser = customers[index - 1];
                currentCart = new ShoppingCart(currentUser.Id); // New cart per session
                CustomerWorkflow();
            }
            else
            {
                Console.WriteLine("Invalid user.");
                Console.ReadKey();
            }
        }

        static void CustomerWorkflow()
        {
            bool back = false;
            while (!back)
            {
                Console.Clear();
                Console.WriteLine($"Logged in as: {currentUser.Username}");
                // Console.WriteLine($"Cart Total: {currentCart.CalculateTotal()}"); // Removed as CalculateTotal moved to logic layers
                Console.WriteLine("1. View Products & Add to Cart");
                Console.WriteLine("2. View Cart");
                Console.WriteLine("3. Checkout");
                Console.WriteLine("0. Logout");
                Console.Write("Option: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        AddToMenu();
                        break;
                    case "2":
                        ViewCart();
                        break;
                    case "3":
                        Checkout();
                        break;
                    case "0":
                        back = true;
                        currentUser = null;
                        currentCart = null;
                        break;
                }
            }
        }

        static void AddToMenu()
        {
            Console.Clear();
            var products = productRepo.GetAll().ToList();
            for (int i = 0; i < products.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {products[i].Name} - {products[i].Price}");
            }
            Console.WriteLine("0. Back");
            Console.Write("Select product number to add: ");

            if (int.TryParse(Console.ReadLine(), out int pid) && pid > 0 && pid <= products.Count)
            {
                currentCart.AddProduct(products[pid - 1].Id);
                Console.WriteLine("Added to cart!");
            }
            // Simple pause
            Console.ReadKey();
        }

        static void ViewCart()
        {
            Console.Clear();
            decimal total = 0;
            foreach (var id in currentCart.ProductIds)
            {
                var p = productRepo.GetById(id);
                if (p != null)
                {
                    Console.WriteLine($"{p.Name} - {p.Price}");
                    total += p.Price;
                }
            }
            Console.WriteLine($"Total: {total}");
            Console.ReadKey();
        }

        static void Checkout()
        {
            Console.Clear();
            if (!currentCart.ProductIds.Any())
            {
                Console.WriteLine("Cart is empty.");
                Console.ReadKey();
                return;
            }

            var products = new List<Product>();
            foreach (var id in currentCart.ProductIds)
            {
                var p = productRepo.GetById(id);
                if (p != null) products.Add(p);
            }

            // Applying discount strategy
            IDiscountStrategy discount = new FixedAmountDiscountStrategy(50); // $50 off
            OrderService orderService = new OrderService(discount);

            Order order = orderService.PlaceOrder(currentUser, products);
            orderRepo.Add(order);

            // Update Customer history
            if (currentUser is Customer c)
            {
                c.OrderHistory.Add(order);
            }

            Console.WriteLine($"Order placed! Order ID: {order.Id}");
            Console.WriteLine($"Final Total (after $50 discount): {order.Total}");
            Console.WriteLine($"Status: {order.Status}");

            currentCart.Clear();
            Console.ReadKey();
        }

        static void AdminMenu()
        {
            Console.Clear();
            Console.WriteLine("Admin Menu");
            Console.WriteLine("1. Add Product");
            Console.WriteLine("2. View All Orders");
            Console.WriteLine("0. Back");

            switch (Console.ReadLine())
            {
                case "1":
                    AddProductAdmin();
                    break;
                case "2":
                    ViewOrdersAdmin();
                    break;
                case "0":
                    break;
            }
        }

        static void AddProductAdmin()
        {
            Console.Write("Enter product name: ");
            string name = Console.ReadLine();
            Console.Write("Enter price: ");
            decimal price = decimal.Parse(Console.ReadLine());

            // Use Factory
            ProductFactory factory = new ElectronicProductFactory(); // Standardizing electronics for demo
            var p = factory.CreateProduct(name, price);
            productRepo.Add(p);
            Console.WriteLine("Product added.");
            Console.ReadKey();
        }

        static void ViewOrdersAdmin()
        {
            var orders = orderRepo.GetAll();
            foreach (var o in orders)
            {
                Console.WriteLine($"Order {o.Id} by {o.User.Username} - Total: {o.Total} - Status: {o.Status}");
            }
            Console.ReadKey();
        }
    }
}
