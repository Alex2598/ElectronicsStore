using Store.Web.Areas.Account.Models;
using Store.Web.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

namespace Store.Web.Areas.Account.Controllers
{
    [Area("Account")]
	public class AccountController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly EmailHelper _emailHelper;


        public AccountController(UserManager<ApplicationUser> manager, SignInManager<ApplicationUser> signInManager, EmailHelper emailHelper)
		{
			_userManager = manager;
			_signInManager = signInManager;
			_emailHelper = emailHelper;
		}

		[Route("{area}/{action}")]
		public async Task<IActionResult> SignOut()
		{
			await _signInManager.SignOutAsync();
			return LocalRedirect("/");
		}

		[Route("{area}/{action}")]
		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> SignIn(LoginModel model)
		{
            ApplicationUser user = await _userManager.FindByNameAsync(model.Login);
            bool emailStatus = await _userManager.IsEmailConfirmedAsync(user);
            if (emailStatus == false)
            {
                ModelState.AddModelError(nameof(user.Email), "Email не подтвержден. Пожалуйста, пройдите по ссылке из отправленного вам письма");
                return View("Login");
            }

            var result = await _signInManager.PasswordSignInAsync(model.Login, model.Password, false, lockoutOnFailure: false);
            if (result.Succeeded)
			{
				   return LocalRedirect("/");
			}
            
			if (result.RequiresTwoFactor)
            {
                return RedirectToAction("LoginTwoStep", new { user.Email });
            }
            ModelState.AddModelError("", "Неверный логин или пароль");
			return View("Login");
		}

        [AllowAnonymous]
		[HttpGet]
        [Route("{area}/{action}")]
        public async Task<IActionResult> LoginTwoStep(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
            bool emailResponse = _emailHelper.SendEmailTwoFactorCode(user.Email, token);
            return View(new TwoFactor());
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("{area}/{action}")]
        public async Task<IActionResult> LoginTwoStep(TwoFactor twoFactor)
        {
            if (!ModelState.IsValid)
            {
                return View(twoFactor.TwoFactorCode);
            }

            var result = await _signInManager.TwoFactorSignInAsync("Email", twoFactor.TwoFactorCode, false, false);
            if (result.Succeeded)
            {
                return Redirect(twoFactor.ReturnUrl ?? "/");
            }
            else
            {
                ModelState.AddModelError("", "Неверный код подтверждения");
                return View(twoFactor);
            }
        }



        [Route("{area}/{action}")]
		[HttpGet]
		public IActionResult SignIn()
		{
			return View("Login");
		}


		[Route("{area}/{action}")]
		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}

		[Route("{area}/{action}")]
		[HttpPost]
		public async Task<IActionResult> Register(RegisterModel model)
		{
			var user = model.GetUser();
			IdentityResult result = await _userManager.CreateAsync(user, model.Password);
			if (result.Succeeded)
			{
				var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
				var confirmationLink = Url.Action("ConfirmEmail", "Account", new { token, email = user.Email }, Request.Scheme);
				//EmailHelper emailHelper = new();
				bool emailResponse = _emailHelper.SendEmail(user.Email, confirmationLink);

				if (emailResponse)
				{
					user = await _userManager.FindByEmailAsync(user.Email);
					//добавляем дату рождения в Claimd пользователя
					await _userManager.AddClaimAsync(user, new Claim("Birthday", model.Birthday.ToString("dd.MM.yyyy"), ClaimValueTypes.String));
					return RedirectToAction("Index", "Home");
				}

				else
				{
					ModelState.AddModelError("", "На сервере произошла ошибка. Попробуйте выполнить регистрацию позднее");
					return View();
				}
			}
			else
			{
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
				return View();
			}
		}

        [Route("{area}/{action}")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
		{
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null)
				return View("Error");

			var result = await _userManager.ConfirmEmailAsync(user, token);
			return View(result.Succeeded ? "ConfirmEmail" : "Error");
		}

        [Route("{area}/{action}/{name}")]
		[HttpGet]
        public async Task<IActionResult> UpdateUser(string name)
		{
			var user = await _userManager.FindByNameAsync(name);
			if (user == null)
				return BadRequest();
			return View(user);
		}
        [Route("{area}/{action}")]
        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [Route("{area}/{action}")]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword( string email)
        {
            if (!ModelState.IsValid)
                return View(email);
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return View();
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var url = Url.Action("ResetPassword", "Account", new { user.Email, token }, Request.Scheme);
            bool result = _emailHelper.SendEmailResetPassword(user.Email, url);
            if (result == false)
            {
                return BadRequest();
            }
            else
                return RedirectToAction("ForgotPasswordConfirmation");
        }
        [Route("{area}/{action}")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPassword)
        {
            if (!ModelState.IsValid)
                return View(resetPassword);
            var user = await _userManager.FindByEmailAsync(resetPassword.Email);
            if (user == null)
                return View(resetPassword);
            var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);
            if (!resetPassResult.Succeeded)
            {
                foreach (var error in resetPassResult.Errors)
                    ModelState.AddModelError(error.Code, error.Description);
                return View();
            }
            return RedirectToAction("ResetPasswordConfirmation");
        }
        [Route("{area}/{action}")]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        [Route("{area}/{action}")]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPasswordModel { Token = token, Email = email };
            return View(model);
        }
        [Route("{area}/{action}")]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        [Route("{area}/{action}/{name}")]
		[HttpPost]
		public async Task<IActionResult> UpdateUser(ApplicationUser user)
		{
			var updatedUser = await _userManager.FindByIdAsync(user.Id.ToString());
			if (updatedUser == null)
				return BadRequest();
			updatedUser.TwoFactorEnabled = user.TwoFactorEnabled;
			await _userManager.UpdateAsync(updatedUser);
			return LocalRedirect("/");
		}
	}
}
