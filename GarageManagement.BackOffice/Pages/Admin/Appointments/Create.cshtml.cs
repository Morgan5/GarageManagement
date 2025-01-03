using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using GarageManagement.BackOffice.Data;
using GarageManagement.BackOffice.Models;
using Microsoft.AspNetCore.Authorization;

namespace GarageManagement.BackOffice.Pages.Admin.Appointments
{
    [Authorize(Policy = "AdminOnly")]
    public class CreateModel : PageModel
    {
        private readonly GarageManagement.BackOffice.Data.GarageManagementDbContext _context;

        public CreateModel(GarageManagement.BackOffice.Data.GarageManagementDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["VehicleId"] = new SelectList(_context.Vehicle, "Id", "Immatriculation");
            return Page();
        }

        [BindProperty]
        public Appointment Appointment { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            // Vérification de la cohérence des dates
            if (Appointment.ExpectedAt < DateTime.Now)
            {
                ModelState.AddModelError(string.Empty, "La date prévue ne peut pas être avant la date du jour.");
                ViewData["VehicleId"] = new SelectList(_context.Vehicle, "Id", "Immatriculation");
                return Page();
            }

            // Vérification de la disponibilité des rendez-vous
            var existingAppointment = _context.Appointment.Where(a => 
                (Appointment.ExpectedAt == a.ExpectedAt && a.ProgrammedAt == null) ||
                (Appointment.ExpectedAt == a.ProgrammedAt)
            ).FirstOrDefault();
            if (existingAppointment != null)
            {
                ModelState.AddModelError(string.Empty, "Un rendez-vous existe deja dans cet horaire");
                ViewData["VehicleId"] = new SelectList(_context.Vehicle, "Id", "Immatriculation");
                return Page();
            }

            _context.Appointment.Add(Appointment);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
