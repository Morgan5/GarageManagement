using Microsoft.AspNetCore.Mvc;
using GarageManagement.FrontOffice.Services;
using System.Threading.Tasks;
using GarageManagement.FrontOffice.Models;
using Microsoft.Extensions.Configuration;
using System.Linq;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Kernel.Colors;

namespace GarageManagement.FrontOffice.Controllers
{
    public class FactureController : Controller
    {
        private readonly FactureService _factureService;

        public FactureController(FactureService factureService)
        {
            _factureService = factureService;
        }

        // Méthode GET pour afficher les factures disponnibles de chaque réparation
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var factures = await _factureService.GetFacturesByUserId(userId);

            return View(factures);
        }

        // Méthode POST pour télécharger la facture
        [HttpPost]
        public async Task<IActionResult> ExportToPdf(long id)
        {
            var facture = await _factureService.GetFactureById(id);

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