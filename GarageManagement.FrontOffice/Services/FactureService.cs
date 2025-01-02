using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using GarageManagement.FrontOffice.Models;
using Dapper;

namespace GarageManagement.FrontOffice.Services
{
    public class FactureService
    {
        private readonly string _connectionString;

        public FactureService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // fonction liste des factures disponnibles de chaque réparation
        public async Task<IEnumerable<Facturation>> GetFacturesByUserId(long userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"
                    SELECT f.*, 
                        r.*, 
                        v.*, 
                        u.Firstname, 
                        u.Lastname
                    FROM Facturation f
                    INNER JOIN Reparation r ON f.ReparationId = r.Id
                    INNER JOIN Vehicle v ON r.VehicleId = v.Id
                    INNER JOIN [User] u ON v.UserId = u.Id
                    WHERE u.Id = @UserId";

                var factureDictionary = new Dictionary<long, Facturation>();

                var result = await connection.QueryAsync<Facturation, Reparation, Vehicle, Facturation>(
                    sql,
                    (facture, reparation, vehicle) =>
                    {
                        // Associer les entités correctement
                        if (!factureDictionary.TryGetValue(facture.Id, out var currentFacture))
                        {
                            currentFacture = facture;
                            currentFacture.Reparation = reparation;
                            currentFacture.Reparation.Vehicle = vehicle;
                            factureDictionary.Add(currentFacture.Id, currentFacture);
                        }

                        return currentFacture;
                    },
                    new { UserId = userId },
                    splitOn: "Id,Id"
                );

                return factureDictionary.Values;
            }
        }

        // fonction détail pdf de la facture
        public async Task<Facturation> GetFactureById(long id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"
                    SELECT f.*, 
                        r.*, 
                        v.*, 
                        u.*, 
                        fd.*
                    FROM Facturation f
                    INNER JOIN Reparation r ON f.ReparationId = r.Id
                    INNER JOIN Vehicle v ON r.VehicleId = v.Id
                    INNER JOIN [User] u ON v.UserId = u.Id
                    LEFT JOIN FacturationDetail fd ON f.Id = fd.FacturationId
                    WHERE f.Id = @Id;
                ";

                var factureDictionary = new Dictionary<long, Facturation>();

                var result = await connection.QueryAsync<Facturation, Reparation, Vehicle, User, FacturationDetail, Facturation>(
                    sql,
                    (facture, reparation, vehicle, user, detail) =>
                    {
                        // Associer la facture principale
                        if (!factureDictionary.TryGetValue(facture.Id, out var currentFacture))
                        {
                            currentFacture = facture;
                            currentFacture.Reparation = reparation;
                            currentFacture.Reparation.Vehicle = vehicle;
                            currentFacture.Reparation.Vehicle.User = user;
                            currentFacture.FacturationDetails = new List<FacturationDetail>();
                            factureDictionary.Add(currentFacture.Id, currentFacture);
                        }

                        // Ajouter les détails de facturation s'ils existent
                        if (detail != null)
                        {
                            currentFacture.FacturationDetails.Add(detail);
                        }

                        return currentFacture;
                    },
                    new { Id = id },
                    splitOn: "Id,Id,Id,Id"
                );

                return factureDictionary.Values.FirstOrDefault();
            }
        }

    }
}