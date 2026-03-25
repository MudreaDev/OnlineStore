using System.ComponentModel.DataAnnotations;

namespace OnlineStore.WebUI.Models
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Codul este obligatoriu.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Codul trebuie să aibă 6 cifre.")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Parola nouă este obligatorie.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Parola trebuie să aibă cel puțin 6 caractere.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Parolele nu se potrivesc.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
