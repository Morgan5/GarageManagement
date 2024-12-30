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

namespace GarageManagement.BackOffice.Pages.Admin.Reparations
{
    [Authorize(Policy = "AdminOnly")]
    public class DetailsModel : PageModel
    {
        private readonly GarageManagement.BackOffice.Data.GarageManagementDbContext _context;

        public DetailsModel(GarageManagement.BackOffice.Data.GarageManagementDbContext context)
        {
            _context = context;
        }

        public Reparation Reparation { get; set; } = default!;
        public decimal TotalCost { get; set; }
        public List<Facturation> Factures { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reparation = await _context.Reparation
                                .Include(r => r.Vehicle)
                                .Include(r => r.Vehicle.Model)
                                .Include(r => r.Vehicle.Model.Brand)
                                .Include(r => r.Vehicle.User)
                                .Include(r => r.ReparationEmployees)
                                    .ThenInclude(re => re.Employee)
                                .Include(r => r.ReparationDetails)
                                    .ThenInclude(rd => rd.ReparationType)
                                .FirstOrDefaultAsync(m => m.Id == id);

            if (reparation is not null)
            {
                Reparation = reparation;

                // Calculer le prix total
                TotalCost = Reparation.ReparationDetails.Sum(detail =>
                    detail.IsCustomCost ? detail.CustomCost ?? 0 : detail.ReparationType.DefaultCost
                );

                // Liste des factures
                Factures = await _context.Facturation
                            .Include(f => f.Reparation)
                            .ThenInclude(r => r.Vehicle)
                            .ThenInclude(v => v.User)
                            .Include(f => f.FacturationDetails) 
                            .Where(f => f.ReparationId == id)
                            .ToListAsync();

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostGenerateInvoiceAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reparation = await _context.Reparation
                .Include(r => r.ReparationDetails)
                    .ThenInclude(rd => rd.ReparationType)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reparation == null || reparation.Status != 2 || reparation.EndAt == null)
            {
                return NotFound();
            }

            // Créer une facture
            var facture = new Facturation
            {
                ReparationId = reparation.Id,
                TotalCost = reparation.ReparationDetails.Sum(detail =>
                    detail.IsCustomCost ? detail.CustomCost ?? 0 : detail.ReparationType.DefaultCost
                )
            };

            _context.Facturation.Add(facture);
            await _context.SaveChangesAsync();

            // Ajouter les détails de la facturation
            foreach (var detail in reparation.ReparationDetails)
            {
                var facturationDetail = new FacturationDetail
                {
                    FacturationId = facture.Id,
                    ReparationTypeLabel = detail.ReparationType?.Label,
                    Cost = detail.IsCustomCost ? detail.CustomCost : detail.ReparationType.DefaultCost,
                    CreatedAt = DateTime.Now
                };

                _context.FacturationDetail.Add(facturationDetail);
            }

            await _context.SaveChangesAsync();

            return RedirectToPage(new { id = reparation.Id });
        }

        public async Task<IActionResult> OnGetExportToPdfAsync(long id)
        {
            var facture = await _context.Facturation
                .Include(f => f.Reparation)
                .ThenInclude(r => r.Vehicle)
                .ThenInclude(v => v.User)
                .Include(f => f.FacturationDetails)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (facture == null)
            {
                return NotFound();
            }

            using (var stream = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(stream);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                // Configuration des marges
                document.SetMargins(20, 20, 20, 20);

                // Entête de la facture
                document.Add(new Paragraph("Morgan's Garage")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(24)
                    );

                document.Add(new Paragraph("Rue : Antananarivo")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(12));
                document.Add(new Paragraph("Email : tana@yopmail.com | Téléphone : 0321212312")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(12));
                document.Add(new LineSeparator(new SolidLine()));

                // Informations du destinataire
                var user = facture.Reparation.Vehicle.User;
                document.Add(new Paragraph("Facture")
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetFontSize(20)
                    .SetMarginTop(20));
                document.Add(new Paragraph($"Destinataire : {user.Lastname} {user.Firstname}").SetFontSize(12));
                document.Add(new Paragraph($"Numéro facture : {facture.Id}").SetFontSize(12));
                document.Add(new Paragraph($"Date de facture : {facture.CreatedAt:dd/MM/yyyy}").SetFontSize(12));
                document.Add(new Paragraph($"Immatriculation véhicule : {facture.Reparation.Vehicle.Immatriculation}").SetFontSize(12));
                document.Add(new LineSeparator(new SolidLine()));

                // Table pour les détails de la réparation
                document.Add(new Paragraph("Détails des réparations :")
                    .SetFontSize(16)
                    .SetMarginTop(10));

                Table table = new Table(UnitValue.CreatePercentArray(new float[] { 4, 1 }))
                    .UseAllAvailableWidth()
                    .SetMarginTop(10);

                table.AddHeaderCell(new Cell().Add(new Paragraph("Type de réparation")).SetBackgroundColor(ColorConstants.LIGHT_GRAY));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Prix")).SetBackgroundColor(ColorConstants.LIGHT_GRAY));

                foreach (var detail in facture.FacturationDetails)
                {
                    string label = detail.ReparationTypeLabel ?? "Type inconnu";
                    decimal price = detail.Cost ?? 0;

                    table.AddCell(new Cell().Add(new Paragraph(label)));
                    table.AddCell(new Cell().Add(new Paragraph($"{price:0.00}")));
                }

                document.Add(table);

                // Prix total
                document.Add(new Paragraph($"Prix total : {facture.TotalCost:0.00}")
                    .SetFontSize(14)
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetMarginTop(10));

                document.Add(new LineSeparator(new SolidLine()).SetMarginTop(20));

                // Pied de page
                document.Add(new Paragraph("Merci pour votre confiance.")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(12)
                    .SetMarginTop(20));
                document.Add(new Paragraph("Conditions générales de vente disponibles sur demande.")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(10));

                document.Close();

                // Retourner le fichier pour téléchargement
                return File(stream.ToArray(), "application/pdf", $"Facture_{facture.Id}_{facture.CreatedAt:yyyyMMdd}_{facture.Reparation.Vehicle.Immatriculation}.pdf");
            }

        }
    }
}
