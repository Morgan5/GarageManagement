using Microsoft.EntityFrameworkCore;
using GarageManagement.BackOffice.Data;

namespace GarageManagement.BackOffice.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new GarageManagementDbContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<GarageManagementDbContext>>()))
            {
                if (context == null || context.User == null)
                {
                    throw new ArgumentNullException("Null GarageManagementDbContext");
                }
                
                if (context.User.Any() || context.Brand.Any() || context.Vehicle.Any() || context.Model.Any())
                {
                    return;
                }

                // Ajout des marques
                var brands = new[]
                {
                    new Brand { Label = "Toyota" },
                    new Brand { Label = "Renault" },
                    new Brand { Label = "BMW" },
                    new Brand { Label = "Mercedes" },
                    new Brand { Label = "Ford" }
                };

                context.Brand.AddRange(brands);
                context.SaveChanges();

                // Ajout des modèles
                var models = new[]
                {
                    new Model { Label = "Corolla", BrandId = brands[0].Id },
                    new Model { Label = "Yaris", BrandId = brands[0].Id },
                    new Model { Label = "Clio", BrandId = brands[1].Id },
                    new Model { Label = "Megane", BrandId = brands[1].Id },
                    new Model { Label = "X5", BrandId = brands[2].Id },
                    new Model { Label = "320i", BrandId = brands[2].Id },
                    new Model { Label = "C-Class", BrandId = brands[3].Id },
                    new Model { Label = "E-Class", BrandId = brands[3].Id },
                    new Model { Label = "Fiesta", BrandId = brands[4].Id },
                    new Model { Label = "Focus", BrandId = brands[4].Id }
                };

                context.Model.AddRange(models);
                context.SaveChanges();

                // Ajout des utilisateurs
                var users = new[]
                {
                    new User { IsAdmin = true, Firstname = "Admin", Lastname = "Admin", Birthdate = DateTime.Parse("1989-2-12"), Phone = "123456789", Email = "admin@yopmail.com", Password = "admin" },
                    new User { IsAdmin = false, Firstname = "Jeanne", Lastname = "Rakoto", Birthdate = DateTime.Parse("1995-2-12"), Phone = "0321212312", Email = "jean.rakoto@yopmail.com", Password = "jeanrakoto" },
                    new User { IsAdmin = false, Firstname = "Jean", Lastname = "Ravao", Birthdate = DateTime.Parse("2001-2-12"), Phone = "0341212312", Email = "jeanne.ravao@yopmail.com", Password = "jeanneravao" },
                    new User { IsAdmin = false, Firstname = "Alice", Lastname = "Smith", Birthdate = DateTime.Parse("1990-5-22"), Phone = "0331234567", Email = "alice.smith@yopmail.com", Password = "alicesmith" },
                    new User { IsAdmin = false, Firstname = "Bob", Lastname = "Johnson", Birthdate = DateTime.Parse("1992-7-15"), Phone = "0349876543", Email = "bob.johnson@yopmail.com", Password = "bobjohnson" }
                };

                context.User.AddRange(users);
                context.SaveChanges();

                // Ajout des véhicules
                var vehicles = new[]
                {
                    new Vehicle { Immatriculation = "ABC-123", ModelId = models[0].Id, UserId = users[1].Id },
                    new Vehicle { Immatriculation = "XYZ-456", ModelId = models[2].Id, UserId = users[2].Id },
                    new Vehicle { Immatriculation = "LMN-789", ModelId = models[4].Id, UserId = users[1].Id },
                    new Vehicle { Immatriculation = "OPQ-111", ModelId = models[1].Id, UserId = users[3].Id },
                    new Vehicle { Immatriculation = "RST-222", ModelId = models[5].Id, UserId = users[4].Id },
                    new Vehicle { Immatriculation = "UVW-333", ModelId = models[3].Id, UserId = users[2].Id },
                    new Vehicle { Immatriculation = "GHI-444", ModelId = models[6].Id, UserId = users[3].Id },
                    new Vehicle { Immatriculation = "JKL-555", ModelId = models[7].Id, UserId = users[4].Id },
                    new Vehicle { Immatriculation = "MNO-666", ModelId = models[8].Id, UserId = users[1].Id },
                    new Vehicle { Immatriculation = "PQR-777", ModelId = models[9].Id, UserId = users[2].Id },
                    new Vehicle { Immatriculation = "STU-888", ModelId = models[0].Id, UserId = users[3].Id },
                    new Vehicle { Immatriculation = "VWX-999", ModelId = models[2].Id, UserId = users[4].Id },
                    new Vehicle { Immatriculation = "DEF-222", ModelId = models[1].Id, UserId = users[1].Id },
                    new Vehicle { Immatriculation = "GHI-333", ModelId = models[3].Id, UserId = users[2].Id },
                    new Vehicle { Immatriculation = "JKL-444", ModelId = models[5].Id, UserId = users[3].Id },
                    new Vehicle { Immatriculation = "MNO-555", ModelId = models[7].Id, UserId = users[4].Id },
                    new Vehicle { Immatriculation = "PQR-666", ModelId = models[9].Id, UserId = users[1].Id },
                    new Vehicle { Immatriculation = "STU-777", ModelId = models[0].Id, UserId = users[2].Id },
                    new Vehicle { Immatriculation = "VWX-888", ModelId = models[2].Id, UserId = users[3].Id },
                    new Vehicle { Immatriculation = "XYZ-999", ModelId = models[4].Id, UserId = users[4].Id }
                };

                context.Vehicle.AddRange(vehicles);
                context.SaveChanges();
            }
        }
    }
}
