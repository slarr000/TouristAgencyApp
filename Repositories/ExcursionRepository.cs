using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using TouristAgencyApp.DTOs;
using TouristAgencyApp.Interfaces;
using TouristAgencyApp.Models;

namespace TouristAgencyApp.Repositories
{
    public class ExcursionRepository : IExcursionRepository
    {
        private readonly string _connectionString;

        public ExcursionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("Connection string is missing");
        }

        public async Task<IEnumerable<Excursion>> GetAllAsync()
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            string sql = "SELECT * FROM \"Excursions\"";
            return await db.QueryAsync<Excursion>(sql);
        }

        public async Task<int> AddAsync(ExcursionCreateDto excursion)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);

            string sql = @"
                INSERT INTO ""Excursions"" (agency_id, excursion_name, schedule_date, price) 
                VALUES (@agency_id, @excursion_name, @schedule_date, @price) 
                RETURNING excursion_id;";

            return await db.QuerySingleAsync<int>(sql, excursion);
        }

        // ... существующие методы GetAllAsync и AddAsync ...

        public async Task<Excursion?> GetByIdAsync(int id)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            string sql = "SELECT * FROM \"Excursions\" WHERE excursion_id = @id";

            return await db.QueryFirstOrDefaultAsync<Excursion>(sql, new { id });
        }

        public async Task<bool> UpdateAsync(int id, ExcursionCreateDto excursion)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            string sql = @"
                UPDATE ""Excursions"" 
                SET agency_id = @agency_id, 
                    excursion_name = @excursion_name, 
                    schedule_date = @schedule_date, 
                    price = @price 
                WHERE excursion_id = @id";

            var rowsAffected = await db.ExecuteAsync(sql, new
            {
                id,
                excursion.agency_id,
                excursion.excursion_name,
                excursion.schedule_date,
                excursion.price
            });

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            string sql = "DELETE FROM \"Excursions\" WHERE excursion_id = @id";

            var rowsAffected = await db.ExecuteAsync(sql, new { id });
            return rowsAffected > 0;
        }


    }
}