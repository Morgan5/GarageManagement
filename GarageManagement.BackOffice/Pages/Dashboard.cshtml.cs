using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using GarageManagement.BackOffice.Models;
using Microsoft.AspNetCore.Mvc;

namespace GarageManagement.BackOffice.Pages
{
    [Authorize(Policy = "AdminOnly")]
    public class DashboardModel : PageModel
    {
        private readonly GarageManagement.BackOffice.Data.GarageManagementDbContext _context;

        public DashboardModel(GarageManagement.BackOffice.Data.GarageManagementDbContext context)
        {
            _context = context;
        }

        public List<int> AvailableYears { get; set; }
        public List<int> AvailableYearsReparation { get; set; }
        public List<int> AvailableYearsReparationEmployee { get; set; }
        public int CurrentYear { get; set; }
        public int CurrentMonth { get; set; }
        public Dictionary<string, int> AppointmentStats { get; set; }
        public Dictionary<string, int> ReparationTypeStats { get; set; }
        public Dictionary<string, int> ReparationPeriodStats { get; set; }
        public Dictionary<string, Dictionary<string, int>> EmployeeStats { get; set; }

        public async Task LoadAppointmentStats(int? year = null)
        {
            var selectedYear = year ?? DateTime.Now.Year;

            AppointmentStats = await _context.Appointment
                .Where(a => a.CreatedAt.Year == selectedYear)
                .GroupBy(a => a.CreatedAt.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .OrderBy(x => x.Month)
                .ToDictionaryAsync(
                    x => System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Month), 
                    x => x.Count
                );
        }

        public async Task<IActionResult> OnGetAppointmentStatsAsync(int year)
        {
            // Regroupe les statistiques par mois pour l'année donnée
            var stats = await _context.Appointment
                .Where(a => a.CreatedAt.Year == year)
                .GroupBy(a => a.CreatedAt.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .OrderBy(x => x.Month)
                .ToDictionaryAsync(
                    x => System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Month), 
                    x => x.Count
                );

            return new JsonResult(stats);
        }

        public async Task LoadReparationTypeStats()
        {
            // Statistiques sur les types de pannes fréquentes
            ReparationTypeStats = await _context.ReparationDetail
                .GroupBy(rd => rd.ReparationType.Label)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToDictionaryAsync(x => x.Type, x => x.Count);
        }

        public async Task LoadReparationPeriodStats(int? year = null)
        {
            var selectedYear = year ?? DateTime.Now.Year;

            // Statistiques sur les réparations par période
            ReparationPeriodStats = await _context.Reparation
                .Where(a => a.CreatedAt.Year == selectedYear)
                .GroupBy(r => r.CreatedAt.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() }) // Utiliser "Month" explicitement
                .OrderBy(x => x.Month)
                .ToDictionaryAsync(
                    x => System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Month), 
                    x => x.Count
                );
        }

        public async Task<IActionResult> OnGetReparationPeriodStatsAsync(int year)
        {
            // Regroupe les statistiques par mois pour l'année donnée
            var stats = await _context.Reparation
                .Where(a => a.CreatedAt.Year == year)
                .GroupBy(a => a.CreatedAt.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .OrderBy(x => x.Month)
                .ToDictionaryAsync(
                    x => System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Month), 
                    x => x.Count
                );

            return new JsonResult(stats);
        }

        /*public async Task LoadEmployeeStats()
        {
            EmployeeStats = await _context.ReparationEmployee
            .Include(re => re.Employee)
            .Include(re => re.Reparation)
            .GroupBy(re => new
            {
                EmployeeName = re.Employee.Firstname + " " + re.Employee.Lastname,
                Year = re.Reparation.CreatedAt.Year,
                Month = re.Reparation.CreatedAt.Month
            })
            .Select(g => new
            {
                g.Key.EmployeeName,
                Month = g.Key.Year + "-" + g.Key.Month.ToString("D2"),
                Count = g.Count()
            })
            .GroupBy(x => x.EmployeeName)
            .ToDictionaryAsync(
                g => g.Key,
                g => g.ToDictionary(
                    x => x.Month,
                    x => x.Count
                )
            );
        }*/

        public async Task LoadEmployeeStats(int? year = null, int? month = null)
        {
            var selectedYear = year ?? DateTime.Now.Year;
            var selectedMonth = month ?? DateTime.Now.Month;

            EmployeeStats = await _context.ReparationEmployee
                .Include(re => re.Employee)
                .Include(re => re.Reparation)
                .Where(a => a.Reparation.CreatedAt.Year == selectedYear && a.Reparation.CreatedAt.Month == selectedMonth)
                .GroupBy(re => new
                {
                    EmployeeName = re.Employee.Firstname + " " + re.Employee.Lastname,
                    Year = re.Reparation.CreatedAt.Year,
                    Month = re.Reparation.CreatedAt.Month
                })
                .Select(g => new
                {
                    g.Key.EmployeeName,
                    Month = g.Key.Year + "-" + g.Key.Month.ToString("D2"),
                    Count = g.Count()
                })
                .GroupBy(x => x.EmployeeName)
                .ToDictionaryAsync(
                    g => g.Key,
                    g => g.ToDictionary(
                        x => x.Month,
                        x => x.Count
                    )
                );
        }

        public async Task<IActionResult> OnGetEmployeeStatsAsync(int year, int month)
        {
            var stats = await _context.ReparationEmployee
                .Include(re => re.Employee)
                .Include(re => re.Reparation)
                .Where(a => a.Reparation.CreatedAt.Year == year && a.Reparation.CreatedAt.Month == month)
                .GroupBy(re => new
                {
                    EmployeeName = re.Employee.Firstname + " " + re.Employee.Lastname,
                    Year = re.Reparation.CreatedAt.Year,
                    Month = re.Reparation.CreatedAt.Month
                })
                .Select(g => new
                {
                    g.Key.EmployeeName,
                    Month = g.Key.Year + "-" + g.Key.Month.ToString("D2"),
                    Count = g.Count()
                })
                .GroupBy(x => x.EmployeeName)
                .ToDictionaryAsync(
                    g => g.Key, // nom de l'employé
                    g => g.ToDictionary(
                        x => x.Month,
                        x => x.Count // nombre de réparation de l'employé
                    )
                );

            return new JsonResult(stats);
        }

        public async Task OnGetAsync()
        {
            AvailableYears = await _context.Appointment
                .Select(a => a.CreatedAt.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();

            AvailableYearsReparation = await _context.Reparation
                .Select(a => a.CreatedAt.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();

            AvailableYearsReparationEmployee = await _context.ReparationEmployee
                .Select(a => a.Reparation.CreatedAt.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();

            CurrentYear = System.DateTime.Now.Year; 
            CurrentMonth = System.DateTime.Now.Month; 

            await LoadAppointmentStats(CurrentYear);
            await LoadReparationTypeStats();
            await LoadReparationPeriodStats(CurrentYear);
            await LoadEmployeeStats(CurrentYear, CurrentMonth);
        }
    }
}