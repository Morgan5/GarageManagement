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
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Kernel.Colors;

namespace GarageManagement.BackOffice.Pages.Admin.Vehicles
{
    [Authorize(Policy = "AdminOnly")]
    public class IndexModel : PageModel
    {
        private readonly GarageManagement.BackOffice.Data.GarageManagementDbContext _context;

        public IndexModel(GarageManagement.BackOffice.Data.GarageManagementDbContext context)
        {
            _context = context;
        }

        public IList<Vehicle> Vehicle { get; set; } = default!;

        // Propriétés de recherche
        [BindProperty(SupportsGet = true)]
        public string? SearchImmatriculation { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchBrand { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchModel { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchClient { get; set; }

        // Propriétés pour la pagination
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        public int TotalPages { get; set; }

        private const int PageSize = 5;

        public async Task OnGetAsync()
        {
            var query = _context.Vehicle
                .Include(v => v.Model)
                .Include(v => v.Model.Brand)
                .Include(v => v.User)
                .AsQueryable();

            // Application des filtres
            if (!string.IsNullOrEmpty(SearchImmatriculation))
            {
                query = query.Where(v => v.Immatriculation.Contains(SearchImmatriculation));
            }

            if (!string.IsNullOrEmpty(SearchBrand))
            {
                query = query.Where(v => v.Model.Brand.Label.Contains(SearchBrand));
            }

            if (!string.IsNullOrEmpty(SearchModel))
            {
                query = query.Where(v => v.Model.Label.Contains(SearchModel));
            }

            if (!string.IsNullOrEmpty(SearchClient))
            {
                query = query.Where(v => v.User.Lastname.Contains(SearchClient) || v.User.Firstname.Contains(SearchClient));
            }
            
            var totalItems = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            // Pagination
            Vehicle = await query
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostExportToPdfAsync()
        {
            IQueryable<Vehicle> vehicleQuery = _context.Vehicle
                .Include(v => v.Model)
                .Include(v => v.Model.Brand)
                .Include(v => v.User);

            if (!string.IsNullOrEmpty(SearchImmatriculation))
            {
                vehicleQuery = vehicleQuery.Where(v => v.Immatriculation.Contains(SearchImmatriculation));
            }
            if (!string.IsNullOrEmpty(SearchBrand))
            {
                vehicleQuery = vehicleQuery.Where(v => v.Model.Brand.Label.Contains(SearchBrand));
            }
            if (!string.IsNullOrEmpty(SearchModel))
            {
                vehicleQuery = vehicleQuery.Where(v => v.Model.Label.Contains(SearchModel));
            }
            if (!string.IsNullOrEmpty(SearchClient))
            {
                vehicleQuery = vehicleQuery.Where(v => v.User.Lastname.Contains(SearchClient) || v.User.Firstname.Contains(SearchClient));
            }

            var vehicles = await vehicleQuery.ToListAsync();

            using (var stream = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(stream);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                // Entête de la liste
                document.Add(new Paragraph("Liste des Véhicules")
                    .SetFontSize(20)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(20));

                // Création de la table
                Table table = new Table(new float[] { 3, 3, 3, 4 }); // Colonnes avec largeur proportionnelle
                table.SetWidth(UnitValue.CreatePercentValue(100)); // Largeur à 100% de la page

                // Style pour les en-têtes
                Cell headerStyle(string text) =>
                    new Cell().Add(new Paragraph(text)

                        .SetFontSize(12)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                        .SetPadding(5));

                table.AddHeaderCell(headerStyle("Immatriculation"));
                table.AddHeaderCell(headerStyle("Marque"));
                table.AddHeaderCell(headerStyle("Modèle"));
                table.AddHeaderCell(headerStyle("Client"));

                // Style pour les cellules
                Cell cellStyle(string text) =>
                    new Cell().Add(new Paragraph(text)
                        .SetFontSize(10)
                        .SetPadding(5)
                        .SetTextAlignment(TextAlignment.LEFT));

                foreach (var vehicle in vehicles)
                {
                    table.AddCell(cellStyle(vehicle.Immatriculation));
                    table.AddCell(cellStyle(vehicle.Model.Brand.Label));
                    table.AddCell(cellStyle(vehicle.Model.Label));
                    table.AddCell(cellStyle($"{vehicle.User.Lastname} {vehicle.User.Firstname}"));
                }

                // Ajout de la table au document
                document.Add(table);

                // Fermer le document
                document.Close();

                // Retourner le fichier pour téléchargement
                return File(stream.ToArray(), "application/pdf", "ListeDesVehicules.pdf");
            }

        }
    }
}