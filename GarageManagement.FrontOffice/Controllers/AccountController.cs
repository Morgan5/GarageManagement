using Microsoft.AspNetCore.Mvc;
using GarageManagement.FrontOffice.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using GarageManagement.FrontOffice.Models;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace GarageManagement.FrontOffice.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserService _userService;

        public AccountController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Authentifier l'utilisateur via le service
            var user = await _userService.AuthenticateAsync(email, password);
            if (user == null)
            {
                ViewBag.ErrorMessage = "Identifiants invalides ou utilisateur non autorisé.";
                return View();
            }

            // Création des claims pour l'utilisateur
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("IsAdmin", user.IsAdmin.ToString()),
                new Claim("UserId", user.Id.ToString()) // Ajouter l'ID utilisateur
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Authentifier l'utilisateur et créer le cookie
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Rediriger vers la page de gestion du compte
            return RedirectToAction("Management");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> Management()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
            {
                return RedirectToAction("Login");
            }

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // Traitement du formulaire de modification du profil
        [HttpPost]
        public async Task<IActionResult> Management(User user)
        {
            var updatedUser = await _userService.UpdateUserAsync(user);
            //Console.WriteLine(updatedUser);
                if (updatedUser != null)
                {
                    //return RedirectToAction("Management", new { userId = updatedUser.Id });
                    return RedirectToAction("Management");
                }
                else
                {   
                    ViewBag.ErrorMessage = "Erreur lors de la mise à jour du profil";
                }

            return View(user);
        }
    }
}