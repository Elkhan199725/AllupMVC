using AllupWebApplication.Areas.Admin.ViewModels;
using AllupWebApplication.Models;
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
}
