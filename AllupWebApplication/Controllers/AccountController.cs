using AllupWebApplication.Areas.Admin.ViewModels;
using AllupWebApplication.Data;
using AllupWebApplication.Models;
using AllupWebApplication.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AllupWebApplication.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AllupDbContext _context;
    private readonly SignInManager<AppUser> _signInManager;

    public AccountController(UserManager<AppUser> userManager, AllupDbContext context, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _context = context;
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
    public async Task<IActionResult> Login(UserLoginViewModel model)
    {
        if (!ModelState.IsValid) return View();

        AppUser user = null;

        user = await _userManager.FindByNameAsync(model.UserName);

        if (user is null)
        {
            ModelState.AddModelError("", "Invalid credentials!");
            return View();
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Invalid credentials!");
            return View();
        }


        return RedirectToAction("index", "home");
    }

    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(UserRegisterViewModel model)
    {
        if (!ModelState.IsValid) return View();

        AppUser user = new AppUser()
        {
            FullName = model.FullName,
            UserName = model.UserName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
        };

        if (_context.Users.Any(x => x.UserName == model.UserName))
        {
            ModelState.AddModelError("UserName", "A user with this username already exists.");
        }

        if (_context.Users.Any(x => x.Email == model.Email))
        {
            ModelState.AddModelError("Email", "A user with this email already exists.");
        }

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
                return View();
            }
        }

        var roleResult = await _userManager.AddToRoleAsync(user, "Member");

        if (!roleResult.Succeeded)
        {
            foreach (var error in roleResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
                return View();
            }
        }

        return RedirectToAction("login");
    }
}
