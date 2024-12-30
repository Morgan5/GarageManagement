using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using GarageManagement.BackOffice.Data;
using GarageManagement.BackOffice.Models;
using Microsoft.AspNetCore.Authorization;

namespace GarageManagement.BackOffice.Pages.Admin.Reparations
{
    [Authorize(Policy = "AdminOnly")]
    public class CreateModel : PageModel
    {
        private readonly GarageManagement.BackOffice.Data.GarageManagementDbContext _context;

        public CreateModel(GarageManagement.BackOffice.Data.GarageManagementDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["VehicleId"] = new SelectList(_context.Vehicle, "Id", "Immatriculation");
            ViewData["RepairTypeList"] = new SelectList(_context.ReparationType, "Id", "Label");
            ViewData["EmployeeList"] = new SelectList(_context.Employee, "Id", "Firstname");
            return Page();
        }

        [BindProperty]
        public Reparation Reparation { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            // Vérification de la cohérence des dates
            if (Reparation.StartAt > Reparation.FinishedAt || Reparation.StartAt < DateTime.Now)
            {
                ModelState.AddModelError(string.Empty, "La date de début ne peut pas être après la date de fin ou avant la date du jour.");
                ViewData["VehicleId"] = new SelectList(_context.Vehicle, "Id", "Immatriculation");
                ViewData["RepairTypeList"] = new SelectList(_context.ReparationType, "Id", "Label");
                ViewData["EmployeeList"] = new SelectList(_context.Employee, "Id", "Firstname");
                return Page();
            }

            // Vérification de la disponibilité des employés
            foreach (var employeeId in Request.Form["Employees[]"])
            {
                var employee = await _context.Employee.FindAsync(long.Parse(employeeId));

                var existingRepair = _context.ReparationEmployee
                    .Where(re => re.EmployeeId == employee.Id &&
                                re.Reparation.StartAt < Reparation.FinishedAt && 
                                re.Reparation.FinishedAt > Reparation.StartAt &&
                                re.Reparation.Status != 2
                    )
                    .FirstOrDefault();

                if (existingRepair != null)
                {
                    ModelState.AddModelError(string.Empty, $"{employee.Lastname} {employee.Firstname} est déjà affecté à une autre réparation pendant cette période.");
                    ViewData["VehicleId"] = new SelectList(_context.Vehicle, "Id", "Immatriculation");
                    ViewData["RepairTypeList"] = new SelectList(_context.ReparationType, "Id", "Label");
                    ViewData["EmployeeList"] = new SelectList(_context.Employee, "Id", "Firstname");
                    return Page();
                }
            }

            _context.Reparation.Add(Reparation);
            await _context.SaveChangesAsync();

            // Ajout des détails de réparation
            var repairTypeIds = Request.Form["RepairTypes[]"].ToArray();
            var customPrices = Request.Form["RepairCustomPrices[]"].ToArray();

            for (int i = 0; i < repairTypeIds.Length; i++)
            {
                var repairTypeId = long.Parse(repairTypeIds[i]);
                var customPrice = customPrices[i];

                var reparationDetail = new ReparationDetail
                {
                    ReparationId = Reparation.Id,
                    ReparationTypeId = repairTypeId,
                    IsCustomCost = !string.IsNullOrEmpty(customPrice),
                    CustomCost = !string.IsNullOrEmpty(customPrice) ? decimal.Parse(customPrice) : (decimal?)null
                };

                _context.ReparationDetail.Add(reparationDetail);
            }


            // Ajout des employés affectés à la réparation
            foreach (var employeeId in Request.Form["Employees[]"])
            {
                var reparationEmployee = new ReparationEmployee
                {
                    ReparationId = Reparation.Id,
                    EmployeeId = long.Parse(employeeId)
                };
                _context.ReparationEmployee.Add(reparationEmployee);
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

    }
}
