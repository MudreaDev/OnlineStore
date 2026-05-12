using System;
using OnlineStore.Domain.Common;

namespace OnlineStore.Domain.Entities
{
    public class UserNotification : Entity
    {
        public Guid UserId { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Type { get; set; } = "info"; // success, info, warning, error

        public UserNotification() { }

        public UserNotification(Guid userId, string message, string type = "info")
        {
            UserId = userId;
            Message = message;
            Type = type;
        }
    }
}
