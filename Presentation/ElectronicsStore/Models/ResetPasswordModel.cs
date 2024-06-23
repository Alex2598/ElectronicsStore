using System.ComponentModel.DataAnnotations;

namespace Store.Web.Models
{
    public class ResetPasswordModel
    {
        [Required]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        public string Email { get; set; }
        public string Token { get; set; }
    }
}