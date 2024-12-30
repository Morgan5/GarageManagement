using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GarageManagement.BackOffice.Pages
{
    [Authorize(Policy = "AdminOnly")]
    public class DashboardModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
