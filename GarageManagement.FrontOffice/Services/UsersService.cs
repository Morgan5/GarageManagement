using System.Data;
using System.Data.SqlClient;
using GarageManagement.FrontOffice.Models;

namespace GarageManagement.FrontOffice.Services
{
    public class UsersService
    {
        private readonly string _connectionString;

        public UsersService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Récupérer tous les utilisateurs
        public async Task<List<User>> GetUsersAsync()
        {
            var users = new List<User>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT * FROM [User]", connection);
                var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    users.Add(new User
                    {
                        Id = reader.GetInt64(reader.GetOrdinal("Id")),
                        IsAdmin = reader.GetBoolean(reader.GetOrdinal("IsAdmin")),
                        Firstname = reader.GetString(reader.GetOrdinal("Firstname")),
                        Lastname = reader.GetString(reader.GetOrdinal("Lastname")),
                        Birthdate = reader.GetDateTime(reader.GetOrdinal("Birthdate")),
                        Phone = reader.GetString(reader.GetOrdinal("Phone")),
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        Password = reader.GetString(reader.GetOrdinal("Password"))
                    });
                }
            }

            return users;
        }

        // Récupérer un utilisateur par son ID
        public async Task<User?> GetUserAsync(long id)
        {
            User? user = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT * FROM [User] WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);

                var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    user = new User
                    {
                        Id = reader.GetInt64(reader.GetOrdinal("Id")),
                        IsAdmin = reader.GetBoolean(reader.GetOrdinal("IsAdmin")),
                        Firstname = reader.GetString(reader.GetOrdinal("Firstname")),
                        Lastname = reader.GetString(reader.GetOrdinal("Lastname")),
                        Birthdate = reader.GetDateTime(reader.GetOrdinal("Birthdate")),
                        Phone = reader.GetString(reader.GetOrdinal("Phone")),
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        Password = reader.GetString(reader.GetOrdinal("Password"))
                    };
                }
            }

            return user;
        }

        // Ajouter un nouvel utilisateur
        public async Task AddUserAsync(User user)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("INSERT INTO [User] (IsAdmin, Firstname, Lastname, Birthdate, Phone, Email, Password) " +
                                            "VALUES (@IsAdmin, @Firstname, @Lastname, @Birthdate, @Phone, @Email, @Password)", connection);

                command.Parameters.AddWithValue("@IsAdmin", user.IsAdmin);
                command.Parameters.AddWithValue("@Firstname", user.Firstname);
                command.Parameters.AddWithValue("@Lastname", user.Lastname);
                command.Parameters.AddWithValue("@Birthdate", user.Birthdate);
                command.Parameters.AddWithValue("@Phone", user.Phone);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@Password", user.Password);

                await command.ExecuteNonQueryAsync();
            }
        }

        // Mettre à jour un utilisateur
        public async Task UpdateUserAsync(User user)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("UPDATE [User] SET IsAdmin = @IsAdmin, Firstname = @Firstname, Lastname = @Lastname, " +
                                            "Birthdate = @Birthdate, Phone = @Phone, Email = @Email, Password = @Password WHERE Id = @Id", connection);

                command.Parameters.AddWithValue("@IsAdmin", user.IsAdmin);
                command.Parameters.AddWithValue("@Firstname", user.Firstname);
                command.Parameters.AddWithValue("@Lastname", user.Lastname);
                command.Parameters.AddWithValue("@Birthdate", user.Birthdate);
                command.Parameters.AddWithValue("@Phone", user.Phone);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@Password", user.Password);
                command.Parameters.AddWithValue("@Id", user.Id);

                await command.ExecuteNonQueryAsync();
            }
        }

        // Supprimer un utilisateur
        public async Task DeleteUserAsync(long id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("DELETE FROM [User] WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);

                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
