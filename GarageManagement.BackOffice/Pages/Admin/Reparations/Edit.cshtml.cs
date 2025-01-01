using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GarageManagement.BackOffice.Data;
using GarageManagement.BackOffice.Models;
using Microsoft.AspNetCore.Authorization;

namespace GarageManagement.BackOffice.Pages.Admin.Reparations
{
    [Authorize(Policy = "AdminOnly")]
    public class EditModel : PageModel
    {
        private readonly GarageManagementDbContext _context;

        public EditModel(GarageManagementDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Reparation Reparation { get; set; } = default!;

        public IList<ReparationDetail> ReparationDetails { get; set; } = new List<ReparationDetail>();
        public IList<ReparationEmployee> ReparationEmployees { get; set; } = new List<ReparationEmployee>();

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
                return NotFound();

            Reparation = await _context.Reparation
                .Include(r => r.ReparationEmployees)
                    .ThenInclude(re => re.Employee)
                .Include(r => r.ReparationDetails)
                    .ThenInclude(rd => rd.ReparationType)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Reparation == null)
                return NotFound();

            ReparationDetails = Reparation.ReparationDetails.ToList();
            ReparationEmployees = Reparation.ReparationEmployees.ToList();

            ViewData["VehicleId"] = new SelectList(_context.Vehicle, "Id", "Immatriculation", Reparation.VehicleId);
            ViewData["RepairTypeList"] = new SelectList(_context.ReparationType, "Id", "Label");
            ViewData["EmployeeList"] = new SelectList(_context.Employee, "Id", "Firstname");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Vérification de la cohérence des dates
            if (Reparation.StartAt > Reparation.FinishedAt || Reparation.StartAt > Reparation.EndAt || Reparation.StartAt < DateTime.Now)
            {
                Reparation = await _context.Reparation
                .Include(r => r.ReparationEmployees)
                    .ThenInclude(re => re.Employee)
                .Include(r => r.ReparationDetails)
                    .ThenInclude(rd => rd.ReparationType)
                .FirstOrDefaultAsync(m => m.Id == Reparation.Id);

                ReparationDetails = Reparation.ReparationDetails.ToList();
                ReparationEmployees = Reparation.ReparationEmployees.ToList();

                ModelState.AddModelError(string.Empty, "La date de début ne peut pas être après la date de fin ou avant la date du jour.");
                ViewData["VehicleId"] = new SelectList(_context.Vehicle, "Id", "Immatriculation", Reparation.VehicleId);
                ViewData["RepairTypeList"] = new SelectList(_context.ReparationType, "Id", "Label");
                ViewData["EmployeeList"] = new SelectList(_context.Employee, "Id", "Firstname");

                return Page();
            }

            // Vérification de la disponibilité des employés
            foreach (var employeeId in Request.Form["Employees[]"])
            {
                var employee = await _context.Employee.FindAsync(long.Parse(employeeId));

                var existingRepair = _context.ReparationEmployee.AsNoTracking()
                    .Where(re => re.EmployeeId == employee.Id &&
                                re.Reparation.StartAt < Reparation.FinishedAt && 
                                re.Reparation.FinishedAt > Reparation.StartAt &&
                                re.Reparation.Status != 2
                    )
                    .FirstOrDefault();

                if (existingRepair != null)
                {
                    var existingRepairForItself = _context.ReparationEmployee.AsNoTracking()
                    .Where(re => re.EmployeeId == employee.Id &&
                                re.Reparation.StartAt < Reparation.FinishedAt && 
                                re.Reparation.FinishedAt > Reparation.StartAt &&
                                re.Reparation.Id == Reparation.Id
                    )
                    .FirstOrDefault();

                    if(existingRepairForItself == null) {
                        Reparation = await _context.Reparation
                        .Include(r => r.ReparationEmployees)
                            .ThenInclude(re => re.Employee)
                        .Include(r => r.ReparationDetails)
                            .ThenInclude(rd => rd.ReparationType)
                        .FirstOrDefaultAsync(m => m.Id == Reparation.Id);

                        ReparationDetails = Reparation.ReparationDetails.ToList();
                        ReparationEmployees = Reparation.ReparationEmployees.ToList();

                        ModelState.AddModelError(string.Empty, $"{employee.Lastname} {employee.Firstname} est déjà affecté à une autre réparation pendant cette période.");
                        ViewData["VehicleId"] = new SelectList(_context.Vehicle, "Id", "Immatriculation", Reparation.VehicleId);
                        ViewData["RepairTypeList"] = new SelectList(_context.ReparationType, "Id", "Label");
                        ViewData["EmployeeList"] = new SelectList(_context.Employee, "Id", "Firstname");

                        return Page();
                    }
                }
            }
            

            // Mettre à jour les propriétés principales
            _context.Attach(Reparation).State = EntityState.Modified;

            // Suppression et recréation des détails et employés associés
            var existingDetails = _context.ReparationDetail.AsNoTracking().Where(rd => rd.ReparationId == Reparation.Id);
            _context.ReparationDetail.RemoveRange(existingDetails);

            var repairTypeIds = Request.Form["RepairTypes[]"].ToArray();
            var customPrices = Request.Form["RepairCustomPrices[]"].ToArray();

            for (int i = 0; i < repairTypeIds.Length; i++)
            {
                var detail = new ReparationDetail
                {
                    ReparationId = Reparation.Id,
                    ReparationTypeId = long.Parse(repairTypeIds[i]),
                    CustomCost = string.IsNullOrEmpty(customPrices[i]) ? null : decimal.Parse(customPrices[i]),
                    IsCustomCost = !string.IsNullOrEmpty(customPrices[i])
                };
                _context.ReparationDetail.Add(detail);
            }

            var existingEmployees = _context.ReparationEmployee.AsNoTracking().Where(re => re.ReparationId == Reparation.Id);
            _context.ReparationEmployee.RemoveRange(existingEmployees);

            foreach (var employeeId in Request.Form["Employees[]"])
            {
                var employee = new ReparationEmployee
                {
                    ReparationId = Reparation.Id,
                    EmployeeId = long.Parse(employeeId)
                };
                _context.ReparationEmployee.Add(employee);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Reparation.Any(r => r.Id == Reparation.Id))
                    return NotFound();
                throw;
            }

            return RedirectToPage("./Index");
        }
    }
}
