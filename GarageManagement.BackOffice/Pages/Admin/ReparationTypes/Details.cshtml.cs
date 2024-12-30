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

namespace GarageManagement.BackOffice.Pages.Admin.ReparationTypes
{
    [Authorize(Policy = "AdminOnly")]
    public class DetailsModel : PageModel
    {
        private readonly GarageManagement.BackOffice.Data.GarageManagementDbContext _context;

        public DetailsModel(GarageManagement.BackOffice.Data.GarageManagementDbContext context)
        {
            _context = context;
        }

        public ReparationType ReparationType { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reparationtype = await _context.ReparationType.FirstOrDefaultAsync(m => m.Id == id);

            if (reparationtype is not null)
            {
                ReparationType = reparationtype;

                return Page();
            }

            return NotFound();
        }
    }
}
