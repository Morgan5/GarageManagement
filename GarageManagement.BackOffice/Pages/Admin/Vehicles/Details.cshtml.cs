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

namespace GarageManagement.BackOffice.Pages.Admin.Vehicles
{
    [Authorize(Policy = "AdminOnly")]
    public class DetailsModel : PageModel
    {
        private readonly GarageManagementDbContext _context;

        public DetailsModel(GarageManagementDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [BindProperty]
        public Vehicle? Vehicle { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Vehicle = await _context.Vehicle
                .Include(v => v.Model.Brand)
                .Include(v => v.Model)
                .Include(v => v.User)
                .Include(v => v.Reparations)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Vehicle == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}