using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using TouristAgencyApp.DTOs;
using TouristAgencyApp.Interfaces;
using TouristAgencyApp.Models;

namespace TouristAgencyApp.Repositories
{
    public class TouristRepository : ITouristRepository
    {
        private readonly string _connectionString;

        public TouristRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("Connection string is missing");
        }

        public async Task<IEnumerable<Tourist>> GetAllAsync()
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            string sql = "SELECT * FROM \"Tourist Information\"";
            return await db.QueryAsync<Tourist>(sql);
        }

        public async Task<int> AddAsync(TouristCreateDto tourist)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);

            // ДОБАВЛЕНЫ НОВЫЕ ПОЛЯ: category, has_visa
            string sql = @"
                INSERT INTO ""Tourist Information"" 
                (tourist_name, tourist_surname, tourist_patronymic, tourist_date_of_birth, tourist_gender, category, has_visa, flight_id) 
                VALUES 
                (@tourist_name, @tourist_surname, @tourist_patronymic, @tourist_date_of_birth, @tourist_gender, @category, @has_visa, @flight_id) 
                RETURNING tourist_id;";

            return await db.QuerySingleAsync<int>(sql, tourist);
        }

        public async Task<Tourist?> GetByIdAsync(int id)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            string sql = "SELECT * FROM \"Tourist Information\" WHERE tourist_id = @id";
            return await db.QueryFirstOrDefaultAsync<Tourist>(sql, new { id });
        }

        public async Task<bool> UpdateAsync(int id, TouristCreateDto tourist)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);

            // ДОБАВЛЕНЫ НОВЫЕ ПОЛЯ: category, has_visa
            string sql = @"
                UPDATE ""Tourist Information"" 
                SET tourist_name = @tourist_name, 
                    tourist_surname = @tourist_surname, 
                    tourist_patronymic = @tourist_patronymic, 
                    tourist_date_of_birth = @tourist_date_of_birth, 
                    tourist_gender = @tourist_gender, 
                    category = @category,
                    has_visa = @has_visa,
                    flight_id = @flight_id
                WHERE tourist_id = @id";

            var parameters = new
            {
                id,
                tourist.tourist_name,
                tourist.tourist_surname,
                tourist.tourist_patronymic,
                tourist.tourist_date_of_birth,
                tourist.tourist_gender,
                tourist.category,
                tourist.has_visa,
                tourist.flight_id
            };

            var rowsAffected = await db.ExecuteAsync(sql, parameters);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            string sql = "DELETE FROM \"Tourist Information\" WHERE tourist_id = @id";
            var rowsAffected = await db.ExecuteAsync(sql, new { id });
            return rowsAffected > 0;
        }

        public async Task<TouristProfileDto?> GetTouristProfileAsync(int id)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);

            string sql = @"
                SELECT * FROM ""Tourist Information"" WHERE tourist_id = @id;
                SELECT * FROM ""Tourist's Passport Details"" WHERE tourist_id = @id;
                SELECT * FROM ""Tourist Children"" WHERE tourist_id = @id;
                SELECT * FROM ""Tourist Cargo"" WHERE tourist_id = @id;
            ";

            using var multi = await db.QueryMultipleAsync(sql, new { id });
            var profile = await multi.ReadSingleOrDefaultAsync<TouristProfileDto>();

            if (profile != null)
            {
                profile.Passport = await multi.ReadSingleOrDefaultAsync<Passport>();
                profile.Children = (await multi.ReadAsync<TouristChild>()).ToList();
                profile.Cargo = (await multi.ReadAsync<TouristCargo>()).ToList();
            }

            return profile;
        }
    }
}