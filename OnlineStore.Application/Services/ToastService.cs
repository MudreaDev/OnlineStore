using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Collections.Generic;
using System.Text.Json;

namespace OnlineStore.Application.Services
{
    /// <summary>
    /// Serviciu pentru gestionarea notificărilor de tip Toast.
    /// </summary>
    public class ToastService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string ToastSessionKey = "PendingToasts";

        public ToastService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void AddToast(string message, string type = "success")
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return;

            var toasts = GetPendingToasts();
            toasts.Add(new ToastMessage { Message = message, Type = type });
            
            session!.SetString(ToastSessionKey, JsonSerializer.Serialize(toasts));
        }

        public List<ToastMessage> GetPendingToasts()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            var json = session?.GetString(ToastSessionKey);
            
            if (string.IsNullOrEmpty(json)) return new List<ToastMessage>();

            var toasts = JsonSerializer.Deserialize<List<ToastMessage>>(json) ?? new List<ToastMessage>();
            session.Remove(ToastSessionKey); // Consumăm notificările
            return toasts;
        }
    }

    public class ToastMessage
    {
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = "success"; // success, error, info
    }
}
