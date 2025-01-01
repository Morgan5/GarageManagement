using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using GarageManagement.FrontOffice.Models;
using Dapper;

namespace GarageManagement.FrontOffice.Services
{
    public class AppointmentService
    {
        private readonly string _connectionString;

        public AppointmentService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> AddClientAppointment(Appointment appointment)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO Appointment (VehicleId, Motif, CreatedAt, ExpectedAt) 
                               VALUES (@VehicleId, @Motif, @CreatedAt, @ExpectedAt)";
                return await connection.ExecuteAsync(sql, appointment);
            }
        }
    }
}