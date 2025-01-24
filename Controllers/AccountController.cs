using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace proctos.Controllers
{
    public class AccountController : Controller
    {
        // Выход из аккаунта
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Aut", "Home");
        }
    }
}