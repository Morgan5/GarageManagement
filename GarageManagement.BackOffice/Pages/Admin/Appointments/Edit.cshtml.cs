using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GarageManagement.BackOffice.Data;
using GarageManagement.BackOffice.Models;
using Microsoft.AspNetCore.Authorization;

namespace GarageManagement.BackOffice.Pages.Admin.Appointments
{
    [Authorize(Policy = "AdminOnly")]
    public class EditModel : PageModel
    {
        private readonly GarageManagement.BackOffice.Data.GarageManagementDbContext _context;

        public EditModel(GarageManagement.BackOffice.Data.GarageManagementDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Appointment Appointment { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment =  await _context.Appointment.FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }
            Appointment = appointment;
            ViewData["VehicleId"] = new SelectList(_context.Vehicle, "Id", "Immatriculation");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            // Vérification de la cohérence des dates
            if (Appointment.ExpectedAt > Appointment.ProgrammedAt)
            {
                ModelState.AddModelError(string.Empty, "La date prévue ne peut pas être après la date de programmation.");
                ViewData["VehicleId"] = new SelectList(_context.Vehicle, "Id", "Immatriculation");
                return Page();
            }
            if (Appointment.ProgrammedAt < DateTime.Now)
            {
                ModelState.AddModelError(string.Empty, "La date de programmation ne peut pas avoir lieu avant la date du jour.");
                ViewData["VehicleId"] = new SelectList(_context.Vehicle, "Id", "Immatriculation");
                return Page();
            }

            // Vérification de la disponibilité des rendez-vous
            var existingAppointment = _context.Appointment.AsNoTracking().Where(a => a.ProgrammedAt == Appointment.ProgrammedAt).FirstOrDefault();
            if (existingAppointment != null)
            {
                var existingAppointmentForItself = _context.Appointment.AsNoTracking()
                .Where(a => a.ProgrammedAt == Appointment.ProgrammedAt && 
                            a.Id == Appointment.Id
                )
                .FirstOrDefault();
                
                if(existingAppointmentForItself == null) {
                    ModelState.AddModelError(string.Empty, "Un rendez-vous existe deja dans cet horaire");
                    ViewData["VehicleId"] = new SelectList(_context.Vehicle, "Id", "Immatriculation");
                    return Page();
                }
            }

            _context.Attach(Appointment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentExists(Appointment.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool AppointmentExists(long id)
        {
            return _context.Appointment.Any(e => e.Id == id);
        }
    }
}
