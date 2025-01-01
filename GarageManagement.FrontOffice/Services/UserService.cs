using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using GarageManagement.FrontOffice.Models;
using Dapper;

namespace GarageManagement.FrontOffice.Services
{
    public class UserService
    {
        private readonly string _connectionString;

        public UserService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT * FROM [User] WHERE Email = @Email AND Password = @Password AND IsAdmin = 0", connection);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", password);
                
                var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new User
                    {
                        Id = reader.GetInt64(reader.GetOrdinal("Id")), // Utiliser GetInt64 ici
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        Password = reader.GetString(reader.GetOrdinal("Password")),
                        IsAdmin = reader.GetBoolean(reader.GetOrdinal("IsAdmin"))
                    };
                }

                return null;
            }
        }

        // Mettre à jour un utilisateur
        public async Task<User?> UpdateUserAsync(User user)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("UPDATE [User] SET Firstname = @Firstname, Lastname = @Lastname, Birthdate = @Birthdate, Phone = @Phone, Email = @Email, Password = @Password WHERE Id = @Id", connection);

                command.Parameters.AddWithValue("@Id", user.Id);
                command.Parameters.AddWithValue("@Firstname", user.Firstname);
                command.Parameters.AddWithValue("@Lastname", user.Lastname);
                command.Parameters.AddWithValue("@Birthdate", user.Birthdate);
                command.Parameters.AddWithValue("@Phone", user.Phone);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@Password", user.Password);

                var result = await command.ExecuteNonQueryAsync();
                if (result > 0)
                {
                    return user;
                }
                return null;
            }
        }

        public async Task<User?> GetUserByIdAsync(long userId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var queryUserAndAppointments = @"
                SELECT * FROM [User] WHERE Id = @UserId;
                SELECT * FROM Appointment WHERE VehicleId IN (SELECT Id FROM Vehicle WHERE UserId = @UserId);
                SELECT * FROM Vehicle WHERE UserId = @UserId;";

            var multiUserData = await connection.QueryMultipleAsync(queryUserAndAppointments, new { UserId = userId });

            var user = await multiUserData.ReadSingleOrDefaultAsync<User>();
            if (user != null)
            {
                user.Appointments = (await multiUserData.ReadAsync<Appointment>()).ToList();
                user.Vehicles = (await multiUserData.ReadAsync<Vehicle>()).ToList();
            }

            var queryReparations = @"
                SELECT r.*, v.* FROM Reparation r
                JOIN Vehicle v ON v.Id = r.VehicleId
                WHERE v.UserId = @UserId;";

            var reparations = await connection.QueryAsync<Reparation, Vehicle, Reparation>(
                queryReparations,
                (reparation, vehicle) =>
                {
                    reparation.Vehicle = vehicle;
                    return reparation;
                },
                new { UserId = userId });

            if (user != null)
            {
                user.Reparations = reparations.ToList();
            }

            return user;
        }

    }
}