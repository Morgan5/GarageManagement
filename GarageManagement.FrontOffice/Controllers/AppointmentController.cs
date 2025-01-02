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

            return RedirectToAction("Index");
        }
    }
}