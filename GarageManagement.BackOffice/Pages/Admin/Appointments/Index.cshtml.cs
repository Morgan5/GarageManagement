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
    public class IndexModel : PageModel
    {
        private readonly GarageManagement.BackOffice.Data.GarageManagementDbContext _context;

        public IndexModel(GarageManagement.BackOffice.Data.GarageManagementDbContext context)
        {
            _context = context;
        }

        public IList<Appointment> Appointment { get;set; } = default!;

        // Propriétés pour la pagination
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        public int TotalPages { get; set; }

        private const int PageSize = 5;

        // Propriétés pour le tri
        [BindProperty(SupportsGet = true)]
        public string SortColumn { get; set; } = "CreatedAt"; // Colonne par défaut

        [BindProperty(SupportsGet = true)]
        public bool IsAscending { get; set; } = false; // Ordre décroissant par défaut


        public async Task OnGetAsync()
        {
            var query = _context.Appointment
                .Include(a => a.Vehicle)
                .AsQueryable();

            var totalItems = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            // Appliquer le tri
            query = SortColumn switch
            {
                "CreatedAt" => IsAscending ? query.OrderBy(a => a.CreatedAt) : query.OrderByDescending(a => a.CreatedAt),
                "ExpectedAt" => IsAscending ? query.OrderBy(a => a.ExpectedAt) : query.OrderByDescending(a => a.ExpectedAt),
                "ProgrammedAt" => IsAscending ? query.OrderBy(a => a.ProgrammedAt) : query.OrderByDescending(a => a.ProgrammedAt),
                _ => query.OrderByDescending(a => a.CreatedAt),
            };
            
            // Pagination
            Appointment =  await query
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }
    }
}
