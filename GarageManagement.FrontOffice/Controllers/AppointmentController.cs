using Microsoft.AspNetCore.Mvc;
using GarageManagement.FrontOffice.Services;
using System.Threading.Tasks;
using GarageManagement.FrontOffice.Models;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace GarageManagement.FrontOffice.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly AppointmentService _appointmentService;
        private readonly VehicleService _vehicleService; // Service pour récupérer les véhicules

        public AppointmentController(AppointmentService appointmentService, VehicleService vehicleService)
        {
            _appointmentService = appointmentService;
            _vehicleService = vehicleService;
        }

        // Méthode GET pour afficher le formulaire
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Récupérer la liste des véhicules pour l'utilisateur
            var vehicles = await _vehicleService.GetUserVehicles(userId);

            // Passer les véhicules à la vue
            ViewData["Vehicles"] = vehicles;

            return View();
        }

        // Méthode POST pour enregistrer le rendez-vous
        [HttpPost]
        public async Task<IActionResult> Index(Appointment appointment)
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }

            appointment.CreatedAt = DateTime.Now;

            var (isSuccess, errorMessage) = await _appointmentService.AddClientAppointment(appointment);

            if (!isSuccess)
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                var vehicles = await _vehicleService.GetUserVehicles(userId);
                ViewData["Vehicles"] = vehicles;
                return View(appointment);
            }

            return RedirectToAction("Management", "Account");
        }

        // Redirection vers la modification d'un rendez-vous
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            // Récupérer les modèles de véhicules du client
            var vehicle = await _vehicleService.GetUserVehicles(userId);
            ViewBag.Vehicles = vehicle;

            return View(appointment);
        }

        // Méthode POST pour modifier le rendez-vous
        [HttpPost]
        public async Task<IActionResult> Edit(Appointment appointment)
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Vérification de disponibilité
            var (isSuccess, errorMessage) = await _appointmentService.EditClientAppointment(appointment);

            if (!isSuccess)
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                var vehicle = await _vehicleService.GetUserVehicles(userId);
                ViewBag.Vehicles = vehicle;
                return View(appointment);
            }

            return RedirectToAction("Management", "Account");
        }

    }
}