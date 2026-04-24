using OnlineStore.Domain.Entities;
using System;

namespace OnlineStore.Application.Patterns.TemplateMethod
{
    public class InvoiceGenerator : DocumentGenerator
    {
        protected override void AddHeader(Order order)
        {
            _content.AppendLine($"[INVOICE] Order ID: {order.Id}");
            _content.AppendLine($"[INVOICE] Date: {order.OrderDate:f}");
            _content.AppendLine($"[INVOICE] Customer: {order.User.Username}");
            _content.AppendLine("------------------------------------------------");
        }

        protected override void AddContent(Order order)
        {
            _content.AppendLine("ITEMS:");
            foreach (var item in order.Items)
            {
                _content.AppendLine($" - {item.ProductName}");
                _content.AppendLine($"   {item.Quantity} x {item.UnitPrice:C} = {item.Quantity * item.UnitPrice:C}");
            }
            _content.AppendLine("------------------------------------------------");
        }

        protected override void AddFooter(Order order)
        {
            _content.AppendLine($"SUBTOTAL: {order.Total:C}");
            _content.AppendLine("TOTAL:    " + order.Total.ToString("C"));
            _content.AppendLine("");
            _content.AppendLine("Thank you for your business!");
        }
    }

    public class ShippingLabelGenerator : DocumentGenerator
    {
        protected override void AddHeader(Order order)
        {
            _content.AppendLine("SHIPPING TO:");
            _content.AppendLine(order.User.Username.ToUpper());
        }

        protected override void AddContent(Order order)
        {
            _content.AppendLine($"ADDRESS: {order.ShippingAddress}");
            _content.AppendLine($"PHONE:   {order.PhoneNumber}");
            _content.AppendLine("------------------------------------------------");
            _content.AppendLine("PKG ID:  " + order.Id.ToString()[..8].ToUpper());
        }

        protected override void AddFooter(Order order)
        {
            _content.AppendLine("------------------------------------------------");
            _content.AppendLine("|| ||| || ||| || ||| || ||| ||");
            _content.AppendLine("      TRACKING# 1Z" + order.Id.ToString()[..12].ToUpper());
        }
    }
}
