using System.ComponentModel.DataAnnotations;
namespace Store.Web.Models
{
    public class TwoFactor
    {
        [Required]
        public string TwoFactorCode { get; set; } = "";
        public string ReturnUrl { get; set; } = "/";
    }
}
