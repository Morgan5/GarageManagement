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

        public async Task<(bool isSuccess, string errorMessage)> AddClientAppointment(Appointment appointment)
        {
            if (appointment.ExpectedAt < DateTime.Now)
            {
                return (false, "La date prévue ne peut pas être avant la date du jour.");
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                // Vérification de la disponibilité des rendez-vous
                string checkSql = @"SELECT 1 
                                    FROM Appointment 
                                    WHERE (ExpectedAt = @ExpectedAt AND ProgrammedAt IS NULL) 
                                    OR (ProgrammedAt = @ExpectedAt)";
                var isConflict = await connection.ExecuteScalarAsync<bool>(checkSql, appointment);
                if (isConflict)
                {
                    return (false, "Un rendez-vous existe déjà à cet horaire.");
                }

                // Insertion du rendez-vous
                string insertSql = @"INSERT INTO Appointment (VehicleId, Motif, CreatedAt, ExpectedAt) 
                                    VALUES (@VehicleId, @Motif, @CreatedAt, @ExpectedAt)";
                await connection.ExecuteAsync(insertSql, appointment);
            }

            return (true, null);
        }
    }
}