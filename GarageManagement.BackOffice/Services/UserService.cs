using GarageManagement.BackOffice.Data;
using GarageManagement.BackOffice.Models;
using Microsoft.EntityFrameworkCore;

namespace GarageManagement.BackOffice.Services
{
    public class UserService
    {
        private readonly GarageManagementDbContext _context;

        public UserService(GarageManagementDbContext context)
        {
            _context = context;
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            return await _context.User
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
        }
    }
}
