using AllupWebApplication.Areas.Admin.ViewModels;
using AllupWebApplication.Models;
using AllupWebApplication.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AllupWebApplication.Areas.Admin.Controllers;

[Area("admin")]
public class AuthController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(AdminLoginViewModel model)
    {
        if (!ModelState.IsValid) return View();

        AppUser admin = null;

        admin = await _userManager.FindByNameAsync(model.UserName);

        if (admin is null)
        {
            ModelState.AddModelError("", "Invalid credentials!");
            return View();
        }

        var result = await _signInManager.PasswordSignInAsync(admin,model.Password,false,false);

        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Invalid credentials!");
            return View();
        }


        return RedirectToAction("index", "dashboard");
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);

        
        if (user is not null)
        {
            // Generate a password reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetTokenLink = Url.Action("ResetPasswordAsync", "Auth", new { email = model.Email, token = token }, Request.Scheme);

            // Redirect to the confirmation view
            return RedirectToAction("ForgotPasswordConfirmation");
        }

        else
        {
            ModelState.AddModelError("Email", "User not found");
            return RedirectToAction("ForgotPasswordConfirmation");
        }

    }
    public IActionResult ForgotPasswordConfirmation()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ResetPassword(string token, string email)
    {
        if (token == null || email == null)
        {
            ModelState.AddModelError("", "Invalid password reset token");
        }
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            // Don't reveal that the user does not exist
            return RedirectToAction("ResetPasswordConfirmation");
        }

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
        if (result.Succeeded)
        {
            return RedirectToAction("ResetPasswordConfirmation");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        return View(model);
    }

    public IActionResult ResetPasswordConfirmation()
    {
        return View();
    }
}
