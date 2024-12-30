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

namespace GarageManagement.BackOffice.Pages.Admin.Reparations
{
    [Authorize(Policy = "AdminOnly")]
    public class IndexModel : PageModel
    {
        private readonly GarageManagement.BackOffice.Data.GarageManagementDbContext _context;

        public IndexModel(GarageManagement.BackOffice.Data.GarageManagementDbContext context)
        {
            _context = context;
        }

        public IList<Reparation> Reparation { get;set; } = default!;

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
            var query = _context.Reparation
                .Include(r => r.Vehicle)
                .AsQueryable();

            var totalItems = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            // Appliquer le tri
            query = SortColumn switch
            {
                "CreatedAt" => IsAscending ? query.OrderBy(r => r.CreatedAt) : query.OrderByDescending(r => r.CreatedAt),
                "Status" => IsAscending ? query.OrderBy(r => r.Status) : query.OrderByDescending(r => r.Status),
                "StartAt" => IsAscending ? query.OrderBy(r => r.StartAt) : query.OrderByDescending(r => r.StartAt),
                "FinishedAt" => IsAscending ? query.OrderBy(r => r.FinishedAt) : query.OrderByDescending(r => r.FinishedAt),
                "EndAt" => IsAscending ? query.OrderBy(r => r.EndAt) : query.OrderByDescending(r => r.EndAt),
                _ => query.OrderByDescending(r => r.CreatedAt),
            };

            // Pagination
            Reparation = await query
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }
    }
}
