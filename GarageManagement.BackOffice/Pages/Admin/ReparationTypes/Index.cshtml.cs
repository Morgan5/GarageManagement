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
    public class IndexModel : PageModel
    {
        private readonly GarageManagement.BackOffice.Data.GarageManagementDbContext _context;

        public IndexModel(GarageManagement.BackOffice.Data.GarageManagementDbContext context)
        {
            _context = context;
        }

        public IList<ReparationType> ReparationType { get;set; } = default!;

        public async Task OnGetAsync()
        {
            ReparationType = await _context.ReparationType.ToListAsync();
        }
    }
}