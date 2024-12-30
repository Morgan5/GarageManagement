using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GarageManagement.BackOffice.Data;
using GarageManagement.BackOffice.Models;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace GarageManagement.BackOffice.Pages.Admin.Users
{
    [Authorize(Policy = "AdminOnly")]
    public class IndexModel : PageModel
    {
        private readonly GarageManagement.BackOffice.Data.GarageManagementDbContext _context;

        public IndexModel(GarageManagement.BackOffice.Data.GarageManagementDbContext context)
        {
            _context = context;
        }

        public IList<User> User { get;set; } = default!;

        // Propriétés de recherche
        [BindProperty(SupportsGet = true)]
        public string? SearchPhone { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchEmail { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchClient { get; set; }

        // Propriétés pour la pagination
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        public int TotalPages { get; set; }

        private const int PageSize = 5;

        public async Task OnGetAsync()
        {
            var query = _context.User
                .AsQueryable();

            // Application des filtres
            if (!string.IsNullOrEmpty(SearchPhone))
            {
                query = query.Where(u => u.Phone.Contains(SearchPhone));
            }

            if (!string.IsNullOrEmpty(SearchEmail))
            {
                query = query.Where(u => u.Email.Contains(SearchEmail));
            }

            if (!string.IsNullOrEmpty(SearchClient))
            {
                query = query.Where(u => u.Lastname.Contains(SearchClient) || u.Firstname.Contains(SearchClient));
            }
            
            var totalItems = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            // Pagination
            User = await query
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostImportCsvAsync(IFormFile csvFile)
        {
            if (csvFile == null || csvFile.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Veuillez sélectionner un fichier CSV valide.");
                return Page();
            }

            try
            {
                using var stream = csvFile.OpenReadStream();
                using var reader = new StreamReader(stream);
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    Delimiter = ",",
                    HeaderValidated = null,
                    MissingFieldFound = null
                });

                var importedUsers = csv.GetRecords<User>().ToList();

                foreach (var user in importedUsers)
                {
                    // Vérifiez si l'utilisateur existe déjà pour éviter les doublons
                    var existingUser = await _context.User.FirstOrDefaultAsync(u => u.Email == user.Email);
                    if (existingUser == null)
                    {
                        _context.User.Add(user);
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Les données ont été importées avec succès.";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Erreur lors de l'importation : {ex.Message}");
            }

            return RedirectToPage();
        }
    }
}
