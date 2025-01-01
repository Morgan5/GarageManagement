using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using GarageManagement.FrontOffice.Models;
using Dapper;

namespace GarageManagement.FrontOffice.Services
{
    public class VehicleService
    {
        private readonly string _connectionString;

        public VehicleService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Récupérer les véhicules du client par ID utilisateur
        /*public async Task<IEnumerable<Vehicle>> GetVehiclesByUserIdAsync(long userId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT * FROM Vehicle WHERE UserId = @UserId;
                SELECT * FROM Model WHERE Id IN (SELECT ModelId FROM Vehicle WHERE UserId = @UserId);
                SELECT * FROM Brand WHERE Id IN (SELECT BrandId FROM Model WHERE Id IN (SELECT ModelId FROM Vehicle WHERE UserId = @UserId));
            ";

            using var multi = await connection.QueryMultipleAsync(query, new { UserId = userId });
            
            var vehicles = await multi.ReadAsync<Vehicle>();
            var models = await multi.ReadAsync<Model>();
            var brands = await multi.ReadAsync<Brand>();

            foreach (var vehicle in vehicles)
            {
                vehicle.Model = models.FirstOrDefault(m => m.Id == vehicle.ModelId);
                vehicle.Model.Brand = brands.FirstOrDefault(b => b.Id == vehicle.Model.BrandId);
            }

            return vehicles;
        }*/

        public async Task<IEnumerable<Vehicle>> GetVehiclesByUserIdAsync(long userId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT * FROM Vehicle WHERE UserId = @UserId;
                SELECT * FROM Model WHERE Id IN (SELECT ModelId FROM Vehicle WHERE UserId = @UserId);
                SELECT * FROM Brand WHERE Id IN (SELECT BrandId FROM Model WHERE Id IN (SELECT ModelId FROM Vehicle WHERE UserId = @UserId));
            ";

            using var multi = await connection.QueryMultipleAsync(query, new { UserId = userId });
            
            var vehicles = (await multi.ReadAsync<Vehicle>()).ToList();
            var models = (await multi.ReadAsync<Model>()).ToList();
            var brands = (await multi.ReadAsync<Brand>()).ToList();

            foreach (var vehicle in vehicles)
            {
                vehicle.Model = models.FirstOrDefault(m => m.Id == vehicle.ModelId);
                vehicle.Model.Brand = brands.FirstOrDefault(b => b.Id == vehicle.Model.BrandId);
            }

            return vehicles;
        }

        // Ajouter un véhicule
        public async Task AddVehicle(long userId, Vehicle vehicle)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"INSERT INTO Vehicle (Immatriculation, ModelId, UserId) 
                        VALUES (@Immatriculation, @ModelId, @UserId)";
            
            await connection.ExecuteAsync(query, new 
            {
                vehicle.Immatriculation,
                vehicle.ModelId,
                vehicle.UserId
            });
        }

        // Récupérer tous les modèles
        public async Task<IEnumerable<Model>> GetAllModelsAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var query = "SELECT * FROM Model";
            return await connection.QueryAsync<Model>(query);
        }

        public async Task<List<Vehicle>> GetUserVehicles(long userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM Vehicle WHERE UserId = @UserId";
                return (await connection.QueryAsync<Vehicle>(sql, new { UserId = userId })).AsList();
            }
        }
    }
}