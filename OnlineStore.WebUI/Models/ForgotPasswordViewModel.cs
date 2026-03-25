using System.ComponentModel.DataAnnotations;

namespace OnlineStore.WebUI.Models
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email-ul este obligatoriu.")]
        [EmailAddress(ErrorMessage = "Adresa de email nu este validă.")]
        public string Email { get; set; } = string.Empty;
    }
}
