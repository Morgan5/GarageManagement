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

namespace GarageManagement.BackOffice.Pages.Admin.Users
{
    [Authorize(Policy = "AdminOnly")]
    public class DetailsModel : PageModel
    {
        private readonly GarageManagement.BackOffice.Data.GarageManagementDbContext _context;

        public DetailsModel(GarageManagement.BackOffice.Data.GarageManagementDbContext context)
        {
            _context = context;
        }

        public User User { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var user = await _context.User.FirstOrDefaultAsync(m => m.Id == id);
            var user = await _context.User
                             .Include(u => u.Vehicles)
                             .ThenInclude(v => v.Model) 
                             .ThenInclude(v => v.Brand)
                             .FirstOrDefaultAsync(m => m.Id == id);

            if (user is not null)
            {
                User = user;

                return Page();
            }

            return NotFound();
        }
    }
}
