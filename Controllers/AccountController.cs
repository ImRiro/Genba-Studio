using GenbaStudio.Models.Entities;
using GenbaStudio.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GenbaStudio.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<Usuario> _signIn;
    private readonly UserManager<Usuario> _users;

    public AccountController(SignInManager<Usuario> signIn, UserManager<Usuario> users)
    {
        _signIn = signIn;
        _users = users;
    }

    [HttpGet, AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Home");
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel vm, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid) return View(vm);

        var user = await _users.FindByEmailAsync(vm.Email);
        if (user != null)
        {
            var result = await _signIn.PasswordSignInAsync(user, vm.Password, vm.Recordarme, lockoutOnFailure: true);
            if (result.Succeeded)
                return Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl!) : RedirectToAction("Index", "Home");

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Cuenta bloqueada temporalmente por intentos fallidos. Probá en unos minutos.");
                return View(vm);
            }
        }

        ModelState.AddModelError("", "Email o contraseña incorrectos.");
        return View(vm);
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signIn.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }

    [HttpGet, AllowAnonymous]
    public IActionResult AccessDenied() => View();
}
