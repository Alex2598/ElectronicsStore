using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Store.Web.Models;

using Microsoft.AspNetCore.Identity;

namespace Store.Web.Areas.Account.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage ="Поле Логин обязательно для заполнения")]
        [DisplayName("Логин")]
        public string Login {  get; set; }
        [Required(ErrorMessage = "Поле E-mail обязательно для заполнения")]
        [EmailAddress(ErrorMessage ="Введен некорректный адрес")]
        [DisplayName("E-mail")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Поле Организация обязательно для заполнения")]
        [DisplayName("Организация")]
        public string Organization {  get; set; }
        [Required(ErrorMessage = "Поле Отдел обязательно для заполнения")]
        [DisplayName("Отдел")]
        public string Department { get; set; }
        [Required(ErrorMessage = "Поле День рождения обязательно для заполнения")]
        [DisplayName("День рождения")]
        public DateTime Birthday { get; set; }
        [Required(ErrorMessage = "Поле Пароль обязательно для заполнения")]
        [DisplayName("Пароль")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Поле Пароль ещё раз обязательно для заполнения")]
        [Compare("Password",ErrorMessage = "Пароли не совпадают")]
        [DisplayName("Пароль ещё раз")]
        public string ConfirmPassword { get; set; }

        public ApplicationUser GetUser()
        {
            ApplicationUser user = new()
            {
                //Id = 1,
                Email = Email,
                UserName = Login,
                Organization = Organization,
                Departmenet = Department,
                Birthday = DateOnly.FromDateTime(Birthday)
            };
            return user;
        }
    }
}
