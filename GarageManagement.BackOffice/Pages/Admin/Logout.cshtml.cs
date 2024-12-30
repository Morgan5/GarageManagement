using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GarageManagement.BackOffice.Pages.Admin
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync()
        {
            // Supprime le cookie d'authentification
            await HttpContext.SignOutAsync("CookieAuth");
            
            // Redirige vers la page de connexion
            return RedirectToPage("/Admin/Login");
        }
    }
}