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
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace GarageManagement.FrontOffice.Controllers
{
    public class VehicleController : Controller
    {
        private readonly VehicleService _vehicleService;

        public VehicleController(VehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        // Liste des véhicules du client
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Récupérer les véhicules de l'utilisateur
            var vehicles = await _vehicleService.GetVehiclesByUserIdAsync(userId);
            
            // Récupérer les modèles pour la liste déroulante
            var models = await _vehicleService.GetAllModelsAsync();  // Méthode à implémenter dans VehicleService

            // Créer un ViewModel qui contient les données nécessaires
            var model = new VehicleViewModel
            {
                Vehicles = vehicles,
                Models = models,
                NewVehicle = new Vehicle() 
            };

            return View(model);  // Passer le ViewModel à la vue
        }

        // Ajouter un nouveau véhicule du client
        [HttpPost]
        public async Task<IActionResult> Index(VehicleViewModel model)
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Créer un nouveau véhicule
            var vehicle = new Vehicle
            {
                Immatriculation = model.NewVehicle.Immatriculation,
                ModelId = model.NewVehicle.ModelId,  // ModelId envoyé depuis la vue
                UserId = userId
            };

            // Appeler le service pour ajouter le véhicule
            await _vehicleService.AddVehicle(userId, vehicle);
            
            return RedirectToAction("Index");
        }

        // Redirection vers la modification d'un véhicule du client
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var vehicle = await _vehicleService.GetVehicleByIdAsync(id);

            if (vehicle == null)
            {
                return NotFound();
            }

            // Récupérer les modèles de véhicules
            var models = await _vehicleService.GetAllModelsAsync();
            ViewBag.Models = models;

            return View(vehicle);
        }

        // Modifier un véhicule du client
        [HttpPost]
        public IActionResult Edit(Vehicle vehicle) 
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }

            _vehicleService.UpdateVehicle(vehicle);

            return RedirectToAction("Index");
        }
    }
}
