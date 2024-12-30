using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GarageManagement.BackOffice.Data;
using GarageManagement.BackOffice.Models;
using Microsoft.AspNetCore.Authorization;

namespace GarageManagement.BackOffice.Pages.Admin.Appointments
{
    [Authorize(Policy = "AdminOnly")]
    public class DetailsModel : PageModel
    {
        private readonly GarageManagement.BackOffice.Data.GarageManagementDbContext _context;

        public DetailsModel(GarageManagement.BackOffice.Data.GarageManagementDbContext context)
        {
            _context = context;
        }

        public Appointment Appointment { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointment
                            .Include(v => v.Vehicle)
                            .Include(v => v.Vehicle.Model)
                            .Include(v => v.Vehicle.Model.Brand)
                            .Include(v => v.Vehicle.User)
                            .FirstOrDefaultAsync(v => v.Id == id);
            if (appointment is not null)
            {
                Appointment = appointment;

                return Page();
            }

            return NotFound();
        }
    }
}
